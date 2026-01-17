using System.Collections.Generic;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Artifacts;

public class FoldedCorner : Artifact, IRegisterable
{
    public int counter = 0;
    
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("FoldedCorner", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = ModEntry.Instance.CarterDeck.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/folded_corner.png"))
                .Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "FoldedCorner", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "FoldedCorner", "desc"])
                .Localize
        });
    }

    public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition,
        int handCount)
    {
        if (deck != ModEntry.Instance.CarterDeck.Deck) return;
        counter++;
        if (counter >= 4)
        {
            counter = 0;
            combat.Queue(new AStatus
            {
                status = ModEntry.Instance.StackStatus.Status,
                statusAmount = 1,
                targetPlayer = true,
                artifactPulse = Key()
            });
        }
    }

    public override int? GetDisplayNumber(State s)
    {
        return counter;
    }

    public override List<Tooltip> GetExtraTooltips() =>
    [
        StatusMeta.GetTooltips(ModEntry.Instance.StackStatus.Status, 1)[0]
    ];
}