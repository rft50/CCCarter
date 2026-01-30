using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Carter.Actions;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Cards;

public class TwistingAces : Card
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
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "TwistingAces", "name"]).Localize,
            // Art = ModEntry.Instance.card...
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        var actions = Enumerable.Range(0, upgrade == Upgrade.B ? 3 : 2)
            .Select(CardAction (_) => new ACardCost
            {
                actions =
                [
                    new ADiscount()
                ],
                origin = CardDestination.Deck
            }).ToList();
        if (upgrade == Upgrade.A)
            actions.Add(new ADrawCard { count = 2 });

        return actions;
    }
    
    public override CardData GetData(State state)
    {
        return new CardData
        {
            cost = 1,
            description = ModEntry.Instance.Localizations.Localize(["card", "TwistingAces", upgrade == Upgrade.A ? "descA" : "desc"], new { discount = upgrade == Upgrade.B ? 3 : 2, draw = 2 })
        };
    }
}