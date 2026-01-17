using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Artifacts;

public class MagicGloves : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("MagicGloves", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = ModEntry.Instance.CarterDeck.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/magic_gloves.png"))
                .Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "MagicGloves", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "MagicGloves", "desc"])
                .Localize
        });
    }

    public override void OnTurnEnd(State state, Combat combat)
    {
        if (combat.energy == 0) return;
        combat.Queue(new AStatus
        {
            status = Status.drawNextTurn,
            statusAmount = combat.energy,
            targetPlayer = true,
            artifactPulse = Key()
        });
    }
}