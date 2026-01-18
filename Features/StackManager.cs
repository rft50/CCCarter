using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carter.Features;

internal sealed class StackManager
{
    public StackManager()
    {
        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.TryPlayCard)),
            finalizer: new HarmonyMethod(GetType(), nameof(Combat_TryPlayCard_Finalizer))
        );
    }

    private static void Combat_TryPlayCard_Finalizer(Combat __instance, State s, Card card, bool playNoMatterWhatForFree, bool exhaustNoMatterWhat, ref bool __result)
    {
        if (!__result || exhaustNoMatterWhat) return;
        if (s.ship.Get(ModEntry.Instance.StackStatus.Status) <= 0) return;
        if (ModEntry.Instance.Helper.Content.Cards.IsCardTraitActive(s, card, ModEntry.Instance.Helper.Content.Cards.ExhaustCardTrait)
            || ModEntry.Instance.Helper.Content.Cards.IsCardTraitActive(s, card, ModEntry.Instance.Helper.Content.Cards.SingleUseCardTrait)
            || ModEntry.Instance.Helper.Content.Cards.IsCardTraitActive(s, card, ModEntry.Instance.Helper.Content.Cards.RecycleCardTrait)) return;
        
        if (s.FindCard(card.uuid) == null) return;
        
        s.RemoveCardFromWhereverItIs(card.uuid);
        s.SendCardToDeck(card);
        __instance.QueueImmediate(new AStatus
        {
            status = ModEntry.Instance.StackStatus.Status,
            statusAmount = -1,
            targetPlayer = true
        });
    }
}
