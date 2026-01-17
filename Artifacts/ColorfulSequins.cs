using System.Linq;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Artifacts;

public class ColorfulSequins : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("ColorfulSequins", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = ModEntry.Instance.CarterDeck.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/colorful_sequins.png"))
                .Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "ColorfulSequins", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "ColorfulSequins", "desc"])
                .Localize
        });
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        if (combat.turn != 2) return;
        combat.Queue(state.characters.Select(c => new ASearchForColor
        {
            deck = c.deckType!.Value,
            amount = 1,
            showSpecificTooltip = false,
            omitFromTooltips = true,
            from = CardDestination.Deck
        }));
        Pulse();
    }
}