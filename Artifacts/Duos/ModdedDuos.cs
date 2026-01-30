using System.Collections.Generic;
using System.Reflection;
using Carter.External;
using Carter.Features;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Artifacts.Duos;

public class CarterDynaDuo : Artifact, IDuoArtifact, IDrawDuringTurnHook
{
    private static IDynaApi dynaApi = null!;
    
    public int count = 0;
    
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.ModRegistry.AwaitApi<IDynaApi>("Shockah.Dyna", dynaApi =>
        {
            CarterDynaDuo.dynaApi = dynaApi;
            helper.Content.Artifacts.RegisterArtifact(nameof(CarterDynaDuo), new()
            {
                ArtifactType = typeof(CarterDynaDuo),
                Meta = new()
                {
                    owner = duoApi.DuoArtifactVanillaDeck
                },
                Sprite = helper.Content.Sprites
                    .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/Duo/card_friction.png"))
                    .Sprite,
                Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Dyna", "name"]).Localize,
                Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Dyna", "desc"]).Localize
            });
            duoApi.RegisterDuoArtifact(typeof(CarterDynaDuo),
                [ModEntry.Instance.CarterDeck.Deck, dynaApi.DynaDeck.Deck]);
        });
    }

    public override List<Tooltip> GetExtraTooltips() =>
    [
        StatusMeta.GetTooltips(dynaApi.TempNitroStatus.Status, 1)[0]
    ];

    public override int? GetDisplayNumber(State s)
    {
        return count;
    }

    public void OnDrawDuringTurn(State s, Combat c)
    {
        count++;

        if (count >= 4)
        {
            count -= 4;
            c.Queue(new AStatus
            {
                status = dynaApi.TempNitroStatus.Status,
                statusAmount = 1,
                targetPlayer = true,
                artifactPulse = Key()
            });
        }
    }
}