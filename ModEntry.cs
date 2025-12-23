using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
// using Carter.MidrowObjects;
using Carter.Actions;
using Carter.Cards;
using Carter.External;
// using Carter.Enemies;
using Carter.Features;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework;
using System.Xml.Linq;
using System.Diagnostics;
// using Carter.Artifacts;

namespace Carter;

internal class ModEntry : SimpleMod
{
    internal static ModEntry Instance { get; private set; } = null!;
    internal Harmony Harmony;
    internal IKokoroApi KokoroApi;
    internal IKokoroApi.IV2 KokoroApiV2;
    internal IDeckEntry CarterDeck;
    internal ICardTraitEntry Lighten;
    internal IStatusEntry StackStatus;
    internal Spr cardBottom;
    internal Spr cardTop;
    internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
    internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }

    internal static IReadOnlyList<Type> EnemyTypes { get; } = [
		
    ];


    /*
     * The following lists contain references to all types that will be registered to the game.
     * All cards and artifacts must be registered before they may be used in the game.
     * In theory only one collection could be used, containing all registerable types, but it is seperated this way for ease of organization.
     */

    internal Spr intentCardSpr;
    internal Spr lightenTraitSpr;

    private static List<Type> CarterCommonCardTypes = [
        typeof(ShuffleStep),
        typeof(SwitchShot),
        typeof(CardSharp),
        typeof(CardVanish),
        typeof(Glide),
        typeof(RazzleDazzle),
        typeof(Roundabout),
        typeof(Stack),
        typeof(TrickDraw)
    ];
    private static List<Type> CarterUncommonCardTypes = [
        typeof(Cantrip),
        typeof(Flourish),
        typeof(GlitzAndGlam),
        typeof(Showstopper),
        typeof(Spread)
    ];
    private static List<Type> CarterRareCardTypes = [
        typeof(SleightOfHand)
    ];
    private static List<Type> CarterSpecialCardTypes = [
    ];
    private static IEnumerable<Type> CarterCardTypes =
        CarterCommonCardTypes
            .Concat(CarterUncommonCardTypes)
            .Concat(CarterRareCardTypes)
            .Concat(CarterSpecialCardTypes);

    private static List<Type> CarterCommonArtifacts = [

    ];
    private static List<Type> CarterBossArtifacts = [
    ];
    private static IEnumerable<Type> CarterArtifactTypes =
        CarterCommonArtifacts
            .Concat(CarterBossArtifacts);

    private static IEnumerable<Type> AllRegisterableTypes =
        EnemyTypes.Concat(CarterCardTypes)
            .Concat(CarterArtifactTypes);

    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
    {
        Instance = this;
        Harmony = new Harmony("kelsey.Carter");

        /*
         * Some mods provide an API, which can be requested from the ModRegistry.
         * The following is an example of a required dependency - the code would have unexpected errors if Kokoro was not present.
         * Dependencies can (and should) be defined within the nickel.json file, to ensure proper load mod load order.
         */
        KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!;

        KokoroApiV2 = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!.V2;

        AnyLocalizations = new JsonLocalizationProvider(
            tokenExtractor: new SimpleLocalizationTokenExtractor(),
            localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"i18n/{locale}.json").OpenRead()
        );
        Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
            new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(AnyLocalizations)
        );

        cardBottom = RegisterSprite(package, "assets/Card/Bottom.png").Sprite;
        cardTop = RegisterSprite(package, "assets/Card/Top.png").Sprite;

        Spr lightenTraitSpr = RegisterSprite(package, "assets/icon/lighten.png").Sprite;
        // Debugger.Launch();
        Lighten = helper.Content.Cards.RegisterTrait("kelsey.lighten", new CardTraitConfiguration()
        {
            Icon = (State s, Card? card) => lightenTraitSpr,
            Name = AnyLocalizations.Bind(["traits", "lighten", "name"]).Localize,
            Renderer = null, // (State s, Card? card, Vec vec) => false,
            Tooltips = (State s, Card? card) => [new GlossaryTooltip($"ACardCost")
            {
                Icon = lightenTraitSpr,
                Title = Localizations.Localize(["traits", "lighten", "name"]),
                TitleColor = Colors.cardtrait,
                Description = Localizations.Localize(["traits", "lighten", "desc"])
            }]
        });

        /*
         * A deck only defines how cards should be grouped, for things such as codex sorting and Second Opinions.
         * A character must be defined with a deck to allow the cards to be obtainable as a character's cards.
         */
        Spr CarterFrame = RegisterSprite(package, "assets/border_carter.png").Sprite;

        CarterDeck = helper.Content.Decks.RegisterDeck("Carter", new DeckConfiguration
        {
            Definition = new DeckDef
            {
                /*
                 * This color is used in a few places:
                 * TODO On cards, it dictates the sheen on higher rarities, as well as influences the color of the energy cost.
                 * If this deck is given to a playable character, their name will be this color, and their mini will have this color as their border.
                 */
                color = new Color("00ffff"),

                titleColor = new Color("00ffff")
            },

            DefaultCardArt = StableSpr.cards_colorless,
            BorderSprite = CarterFrame,
            Name = AnyLocalizations.Bind(["character", "name"]).Localize
        });

        RegisterAnimation(package, "neutral", "assets/Animation/carter_neutral_", 4);
        RegisterAnimation(package, "squint", "assets/Animation/carter_squint_", 4);
        Instance.Helper.Content.Characters.V2.RegisterCharacterAnimation(new CharacterAnimationConfigurationV2
        {
            CharacterType = CarterDeck.Deck.Key(),
            LoopTag = "gameover",
            Frames = [
                RegisterSprite(package, "assets/Animation/carter_gameover_0.png").Sprite,
            ]
        });
        Instance.Helper.Content.Characters.V2.RegisterCharacterAnimation(new CharacterAnimationConfigurationV2
        {
            CharacterType = CarterDeck.Deck.Key(),
            LoopTag = "mini",
            Frames = [
                RegisterSprite(package, "assets/Animation/carter_mini_0.png").Sprite,
            ]
        });

        helper.Content.Characters.V2.RegisterPlayableCharacter("Carter", new PlayableCharacterConfigurationV2
        {
            Deck = CarterDeck.Deck,
            BorderSprite = RegisterSprite(package, "assets/char.png").Sprite,
            Starters = new StarterDeck
            {
                cards = [
                    new ShuffleStep(),
                    new SwitchShot()
                ],
                /*
                 * Some characters have starting artifacts, in addition to starting cards.
                 * This is where they would be added, much like their starter cards.
                 * This can be safely removed if you have no starting artifacts.
                 */
                artifacts = [
                ]
            },
            Description = AnyLocalizations.Bind(["character", "desc"]).Localize
        });

        ACardSwap.Spr = RegisterSprite(package, "assets/Icon/swap.png").Sprite;
        ACardCost.SprDiscard = RegisterSprite(package, "assets/Icon/discard_cost_on.png").Sprite;
        ACardCost.SprDiscardOff = RegisterSprite(package, "assets/Icon/discard_cost_off.png").Sprite;
        ACardCost.SprExhaust = RegisterSprite(package, "assets/Icon/exhaust_cost_on.png").Sprite;
        ACardCost.SprExhaustOff = RegisterSprite(package, "assets/Icon/exhaust_cost_off.png").Sprite;
        // AExhaustCard.Spr = RegisterSprite(package, "assets/Icon/exhaustCard.png").Sprite;

        StackStatus = helper.Content.Statuses.RegisterStatus("Stack", new StatusConfiguration
        {
            Definition = new StatusDef
            {
                isGood = true,
                affectedByTimestop = false,
                color = new Color("00ffff"),
                icon = RegisterSprite(package, "assets/Icon/stack.png").Sprite
            },
            Name = AnyLocalizations.Bind(["status", "stack", "name"]).Localize,
            Description = AnyLocalizations.Bind(["status", "stack", "desc"]).Localize
        });

        _ = new LightenManager();
        _ = new StackManager();

        foreach (var type in AllRegisterableTypes)
            AccessTools.DeclaredMethod(type, nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);

    }
    
    /*
     * assets must also be registered before they may be used.
     * Unlike cards and artifacts, however, they are very simple to register, and often do not need to be referenced in more than one place.
     * This utility method exists to easily register a sprite, but nothing prevents you from calling the method used yourself.
     */
    public static ISpriteEntry RegisterSprite(IPluginPackage<IModManifest> package, string dir)
    {
        return Instance.Helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile(dir));
    }

    /*
     * Animation frames are typically named very similarly, only differing by the number of the frame itself.
     * This utility method exists to easily register an animation.
     * It expects the animation to start at frame 0, up to frames - 1.
     * TODO It is advised to avoid animations consisting of 2 or 3 frames.
     */
    public static void RegisterAnimation(IPluginPackage<IModManifest> package, string tag, string dir, int frames)
    {
        Instance.Helper.Content.Characters.V2.RegisterCharacterAnimation(new CharacterAnimationConfigurationV2
        {
            CharacterType = Instance.CarterDeck.Deck.Key(),
            LoopTag = tag,
            Frames = Enumerable.Range(0, frames)
                .Select(i => RegisterSprite(package, dir + i + ".png").Sprite)
                .ToImmutableList()
        });
    }
}
