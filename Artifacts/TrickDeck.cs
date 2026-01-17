using System.Reflection;
using Carter.Actions;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Artifacts;

public class TrickDeck : Artifact, IRegisterable
{
    
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("TrickDeck", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = ModEntry.Instance.CarterDeck.Deck,
                pools = [ArtifactPool.Boss],
                unremovable = true
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/trick_deck.png"))
                .Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "TrickDeck", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "TrickDeck", "desc"])
                .Localize
        });
    }

    public override void OnReceiveArtifact(State state)
    {
        state.ship.baseDraw += 1;
    }

    public override void OnRemoveArtifact(State state)
    {
        state.ship.baseDraw -= 1;
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        combat.Queue(new ACardSelect
        {
            browseAction = new AMoveToBottomOfDraw(),
            browseSource = CardBrowse.Source.Hand
        });
    }
}