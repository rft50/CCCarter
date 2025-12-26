using System.Collections.Generic;
using Carter.Actions;
using Carter.Cards;

namespace Carter.Features;

public class HatTrickManager
{
    public HatTrickManager()
    {
        ModEntry.Instance.Helper.Events.RegisterAfterArtifactsHook("OnPlayerPlayCard", OnPlayerPlayCard);
        ModEntry.Instance.Helper.Events.RegisterAfterArtifactsHook("OnTurnEnd", OnTurnEnd);
        ModEntry.Instance.Helper.Events.RegisterAfterArtifactsHook("OnCombatEnd", OnCombatEnd);
    }

    public static void OnPlayerPlayCard(State state, Combat combat, Card card)
    {
        var count = GetCardPlayCount(state) + 1;
        ModEntry.Instance.Helper.ModData.SetModData(state, "CardPlayCount", count);
        
        var tricks = ModEntry.Instance.Helper.ModData.GetModDataOrDefault<HashSet<int>>(state, "HatTricks", []);
        if (card is HatTrick)
        {
            tricks.Add(card.uuid);
            ModEntry.Instance.Helper.ModData.SetModData(state, "HatTricks", tricks);
        }

        Upgrade? upgradeToSave = count switch
        {
            4 => Upgrade.None,
            3 => Upgrade.A,
            6 => Upgrade.B,
            _ => null
        };
        if (upgradeToSave != null)
        {
            foreach (var trick in tricks)
            {
                var trickCard = state.FindCard(trick);
                if (trickCard == null || trickCard.upgrade == upgradeToSave)
                {
                    combat.Queue(new ATryUnexhaustHatTrick
                    {
                        uuid = trick,
                        count = count
                    });
                }
            }
        }
    }

    public static void OnTurnEnd(State state)
    {
        ModEntry.Instance.Helper.ModData.SetModData(state, "CardPlayCount", 0);
    }

    public static void OnCombatEnd(State state)
    {
        ModEntry.Instance.Helper.ModData.SetModData(state, "CardPlayCount", 0);
        ModEntry.Instance.Helper.ModData.RemoveModData(state, "HatTricks");
    }

    public static int GetCardPlayCount(State state)
    {
        if (state == DB.fakeState)
            state = MG.inst.g.state;
        return ModEntry.Instance.Helper.ModData.GetModDataOrDefault(state, "CardPlayCount", 0);
    }
    
    public static int GetHatTrickLimitForUpgrade(Upgrade upgrade) => upgrade switch
    {
        Upgrade.None => 4,
        Upgrade.A => 3,
        Upgrade.B => 6,
        _ => 0
    };
}