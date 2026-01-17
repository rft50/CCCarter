using System.Linq;
using System.Reflection;
using Carter.Actions;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Artifacts;

public class LightAsAFeather : Artifact, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("LightAsAFeather", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = ModEntry.Instance.CarterDeck.Deck,
                pools = [ArtifactPool.Boss],
                unremovable = true
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/light_as_a_feather.png"))
                .Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "LightAsAFeather", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "LightAsAFeather", "desc"])
                .Localize
        });
        
        ModEntry.Instance.Helper.Content.Cards.OnGetFinalDynamicCardTraitOverrides += (_, traits) =>
        {
            if (!traits.State.EnumerateAllArtifacts().OfType<LightAsAFeather>().Any()) return;
            
            traits.SetOverride(ModEntry.Instance.Lighten, true);
        };

        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.TryPlayCard)),
            prefix: new HarmonyMethod(typeof(LightAsAFeather), nameof(Combat_TryPlayCard_Prefix))
        );
    }

    public static void Combat_TryPlayCard_Prefix(State s, Card card, out bool __state, ref bool exhaustNoMatterWhat)
    {
        __state = false;
        if (s.EnumerateAllArtifacts().OfType<LightAsAFeather>().FirstOrDefault((LightAsAFeather?) null) is not {} artifact) return;

        if (card.GetCurrentCost(s) == 0)
        {
            __state = true;
            exhaustNoMatterWhat = true;
            artifact.Pulse();
        }
    }
}