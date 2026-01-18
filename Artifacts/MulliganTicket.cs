using System.Linq;
using System.Reflection;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Artifacts;

public class MulliganTicket : Artifact, IRegisterable
{
    private static Spr readySpr;
    private static Spr usedSpr;

    public bool used = false;
    
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        readySpr = helper.Content.Sprites
            .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/mulligan_ticket.png"))
            .Sprite;
        usedSpr = helper.Content.Sprites
            .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/mulligan_ticket_off.png"))
            .Sprite;
        
        helper.Content.Artifacts.RegisterArtifact("MulliganTicket", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = ModEntry.Instance.CarterDeck.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = readySpr,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "MulliganTicket", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "MulliganTicket", "desc"])
                .Localize
        });

        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.TryPlayCard)),
            postfix: new HarmonyMethod(typeof(MulliganTicket), nameof(Combat_TryPlayCard_Postfix))
        );
    }

    public static void Combat_TryPlayCard_Postfix(Combat __instance, State s, Card card, bool __result, bool exhaustNoMatterWhat)
    {
        if (!__result || exhaustNoMatterWhat) return;
        if (s.EnumerateAllArtifacts().OfType<MulliganTicket>().FirstOrDefault((MulliganTicket?) null) is not {} artifact) return;
        
        if (artifact.used) return;
        if (ModEntry.Instance.Helper.Content.Cards.IsCardTraitActive(s, card,
                ModEntry.Instance.Helper.Content.Cards.ExhaustCardTrait)) return;
        if (ModEntry.Instance.Helper.Content.Cards.IsCardTraitActive(s, card,
                ModEntry.Instance.Helper.Content.Cards.SingleUseCardTrait)) return;
        
        artifact.used = true;
        s.RemoveCardFromWhereverItIs(card.uuid);
        __instance.QueueImmediate(new AAddCard
        {
            card = card,
            destination = CardDestination.Hand,
            artifactPulse = artifact.Key()
        });
    }

    public override void OnCombatEnd(State state)
    {
        used = false;
    }

    public override Spr GetSprite()
    {
        return used ? usedSpr : readySpr; 
    }
}