using HarmonyLib;

namespace Carter.Features;

// detects additional draws done during the player's turn
public class DrawDuringTurnManager
{
    public DrawDuringTurnManager()
    {
        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.BeginCardAction)),
            prefix: AccessTools.DeclaredMethod(typeof(DrawDuringTurnManager), nameof(Combat_BeginCardAction_Prefix))
        );
        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.SendCardToHand)),
            prefix: AccessTools.DeclaredMethod(typeof(DrawDuringTurnManager), nameof(Combat_SendCardToHand_Prefix))
        );
    }

    private static void Combat_BeginCardAction_Prefix(CardAction a, Combat __instance)
    {
        ModEntry.Instance.Helper.ModData.SetModData(__instance, "ListeningForDraw", true);
        if (a is AStartPlayerTurn)
            ModEntry.Instance.Helper.ModData.SetModData(__instance, "ListeningForDraw", false);
    }

    private static void Combat_SendCardToHand_Prefix(State s, Combat __instance)
    {
        TryCardDrawAnnouncement(s, __instance);
    }

    public static void TryCardDrawAnnouncement(State s, Combat c)
    {
        if (!ModEntry.Instance.Helper.ModData.GetModDataOrDefault<bool>(c, "ListeningForDraw")) return;
        ModEntry.Instance.Helper.ModData.SetModData(c, "ListeningForDraw", false);
        foreach (var artifact in s.EnumerateAllArtifacts())
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (artifact is IDrawDuringTurnHook hook)
                hook.OnDrawDuringTurn(s, c);
        }
    }
}

public interface IDrawDuringTurnHook
{
    void OnDrawDuringTurn(State s, Combat c);
}