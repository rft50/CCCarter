using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Carter.Actions;
using Carter.External;
using Carter.Features;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Cards;

public class Grandstand : Card, IRegisterable
{


    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard(new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = ModEntry.Instance.CarterDeck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Grandstand", "name"]).Localize,
            // Art = ModEntry.Instance.card...
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        return [
            ModEntry.Instance.KokoroApiV2.ActionCosts.MakeCostAction(
                ModEntry.Instance.KokoroApiV2.ActionCosts.MakeResourceCost(upgrade == Upgrade.B ? new ExhaustResource() : new DiscardResource(), upgrade == Upgrade.A ? 2 : 4),
                ModEntry.Instance.KokoroApiV2.ContinueStop.MakeTriggerAction(IKokoroApi.IV2.IContinueStopApi.ActionType.Continue, out var id).AsCardAction
            ).AsCardAction,
            ModEntry.Instance.KokoroApiV2.ContinueStop.MakeFlaggedAction(IKokoroApi.IV2.IContinueStopApi.ActionType.Continue, id, new AStunShip()).AsCardAction
        ];
    }
    
    public override CardData GetData(State state)
    {
        return new CardData
        {
            cost = 1,
            exhaust = true
        };
    }
}