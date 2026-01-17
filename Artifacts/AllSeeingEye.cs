using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Artifacts;

public class AllSeeingEye : Artifact, IRegisterable
{
    public int count = 0;
    
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("AllSeeingEye", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = ModEntry.Instance.CarterDeck.Deck,
                pools = [ArtifactPool.Boss],
                unremovable = true
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/all_seeing_eye.png"))
                .Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "AllSeeingEye", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "AllSeeingEye", "desc"])
                .Localize
        });
    }

    public override void OnReceiveArtifact(State state)
    {
        state.ship.baseDraw -= 1;
    }

    public override void OnRemoveArtifact(State state)
    {
        state.ship.baseDraw += 1;
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        count++;
        if (count >= 2)
        {
            count -= 2;
            combat.Queue(new ACardSelect
            {
                browseAction = new PutDiscardedCardInYourHand(),
                browseSource = CardBrowse.Source.DrawOrDiscardPile
            });
        }
    }

    public override int? GetDisplayNumber(State s)
    {
        return count;
    }
}