using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Carter.Actions;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Cards;

public class Showstopper : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Showstopper", "name"]).Localize,
            // Art = ModEntry.Instance.card...
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        return [
            new ACardCost {
                mode = upgrade == Upgrade.B ? ACardCost.Mode.Exhaust : ACardCost.Mode.Discard,
                count = upgrade == Upgrade.A ? 2 : 4,
                actions = [
                    new AStunShip()
                ]
            },
            new AStunShip() { disabled = true }
            /*ModEntry.Instance.KokoroApiV2.SpoofedActions.MakeAction(
                new AStunShip(),
                new ADummyAction()
                ).AsCardAction*/           
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