using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Artifacts;

public class SecretPocket : Artifact, IRegisterable
{
    public int count = 0;
    
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("SecretPocket", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = ModEntry.Instance.CarterDeck.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/secret_pocket.png"))
                .Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "SecretPocket", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "SecretPocket", "desc"])
                .Localize
        });
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        count++;
        if (count >= 3)
        {
            count -= 3;
            combat.Queue(new ADrawCard
            {
                count = 1,
                artifactPulse = Key()
            });
        }
    }

    public override int? GetDisplayNumber(State s)
    {
        return count;
    }
}