using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Cards;

public class ShuffleStep : Card, IRegisterable
{
    public static Spr left;
    public static Spr right;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        left = helper.Content.Sprites
            .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Card/shuffle_shot_left.png")).Sprite;
        right = helper.Content.Sprites
            .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Card/shuffle_shot_right.png")).Sprite;
        helper.Content.Cards.RegisterCard(new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = ModEntry.Instance.CarterDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "ShuffleStep", "name"]).Localize,
            Art = left
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        return [
            new AMove {
                dir = upgrade == Upgrade.B ? -2 : -1,
                targetPlayer = true
            },
            new ADrawCard {
                count = upgrade == Upgrade.A ? 3 : 2
            }
        ];
    }
    
    public override CardData GetData(State state)
    {
        return new CardData
        {
            cost = 1,
            flippable = true,
            art = flipped ? right : left
        };
    }
}