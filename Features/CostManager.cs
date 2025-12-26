using System.Collections.Generic;
using System.Linq;
using Carter.Actions;
using Carter.External;

namespace Carter.Features;

public class CostManager
{
    public static Spr SprExhaust;
    public static Spr SprDiscard;
    public static Spr SprExhaustOff;
    public static Spr SprDiscardOff;
    
    public CostManager()
    {
        ModEntry.Instance.KokoroApiV2.ActionCosts.RegisterHook(new CarterCostHook());
        ModEntry.Instance.KokoroApiV2.ActionCosts.RegisterResourceCostIcon(new DiscardResource(), SprDiscard, SprDiscardOff);
        ModEntry.Instance.KokoroApiV2.ActionCosts.RegisterResourceCostIcon(new ExhaustResource(), SprExhaust, SprExhaustOff);
    }
}

public class DiscardResource : IKokoroApi.IV2.IActionCostsApi.IResource
{
    public string ResourceKey => "Carter::Discard";
    public int GetCurrentResourceAmount(State state, Combat combat)
    {
        var currentCard = ModEntry.Instance.Helper.ModData.ObtainModData<int?>(combat, "Card");
        return combat.hand.Count(c => c.uuid != currentCard);
    }

    public void Pay(State state, Combat combat, int amount)
    {
        combat.QueueImmediate(new ACardCost
        {
            count = amount,
            destination = CardDestination.Discard
        });
    }

    public IReadOnlyList<Tooltip> GetTooltips(State state, Combat combat, int amount)
    {
        return new ACardCost
        {
            count = amount,
            destination = CardDestination.Discard
        }.GetTooltips(state);
    }
}

public class ExhaustResource : IKokoroApi.IV2.IActionCostsApi.IResource
{
    public string ResourceKey => "Carter::Exhaust";
    public int GetCurrentResourceAmount(State state, Combat combat)
    {
        return combat.hand.Count - 1;
    }

    public void Pay(State state, Combat combat, int amount)
    {
        combat.QueueImmediate(new ACardCost
        {
            count = amount,
            destination = CardDestination.Exhaust
        });
    }

    public IReadOnlyList<Tooltip> GetTooltips(State state, Combat combat, int amount)
    {
        return new ACardCost
        {
            count = amount,
            destination = CardDestination.Exhaust
        }.GetTooltips(state);
    }
}

public class CarterCostHook : IKokoroApi.IV2.IActionCostsApi.IHook
{
    public bool ModifyActionCost(IKokoroApi.IV2.IActionCostsApi.IHook.IModifyActionCostArgs args)
    {
        ModEntry.Instance.Helper.ModData.SetModData(args.Combat, "Card", args.Card?.uuid);
        return false;
    }
}