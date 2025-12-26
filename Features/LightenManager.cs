using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carter.Features;

internal sealed class LightenManager
{
    public LightenManager()
    {
        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.AfterWasPlayed)),
            finalizer: new HarmonyMethod(GetType(), nameof(Card_AfterWasPlayed_Finalizer))
        );
        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.GetCurrentCost)),
            finalizer: new HarmonyMethod(GetType(), nameof(Card_GetCurrentCost_Finalizer))
        );
        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.OnExitCombat)),
            finalizer: new HarmonyMethod(GetType(), nameof(Card_OnExitCombat_Finalizer))
        );
    }


    private static void Card_AfterWasPlayed_Finalizer(Card __instance, State state, Combat c)
    {
        if (!IsLighten(state, __instance))
        {
            return;
        }
        IncrementLightenCount(__instance);
    }
    private static void Card_OnExitCombat_Finalizer(Card __instance, State s, Combat c)
    {
        if (!IsLighten(s, __instance))
        {
            return;
        }
        ResetLightenCount(__instance);
    }
    private static void Card_GetCurrentCost_Finalizer(Card __instance, State s, ref int __result)
    {
        if (!IsLighten(s, __instance))
        {
            return;
        }
        __result = Math.Max(0, __result - GetLightenCount(__instance));
    }

    private static int GetLightenCount(Card card)
    {
        int result;
        if (ModEntry.Instance.Helper.ModData.TryGetModData<int>(card, "Lighten", out result))
        {
            return result;
        }
        ModEntry.Instance.Helper.ModData.SetModData(card, "Lighten", 0);
        return 0;
    }

    private static void IncrementLightenCount(Card card)
    {
        int result = 1;
        if (ModEntry.Instance.Helper.ModData.TryGetModData<int>(card, "Lighten", out result))
        {
            result++;
        }
        ModEntry.Instance.Helper.ModData.SetModData(card, "Lighten", result);
    }
    private static void ResetLightenCount(Card card)
    {
        ModEntry.Instance.Helper.ModData.SetModData(card, "Lighten", 0);
    }

    private static bool IsLighten(State s, Card card)
    {
        return ModEntry.Instance.Helper.Content.Cards.IsCardTraitActive(s, card, ModEntry.Instance.Lighten);
    }
}
