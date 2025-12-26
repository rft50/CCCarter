using System.Collections.Generic;
using System.Reflection;
using Carter.Actions;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Cards;

public class CardThrow : Card, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard(new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = ModEntry.Instance.CarterDeck.Deck,
                rarity = Rarity.rare,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "CardThrow", "name"]).Localize,
            // Art = ModEntry.Instance.card...
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        if (upgrade == Upgrade.B)
            return [
                new AAttack
                {
                    damage = GetDmg(s, 2),
                    fast = true
                },
                new AAttack
                {
                    damage = GetDmg(s, 2),
                    fast = true
                },
                new ACardCost
                {
                    origin = CardDestination.Hand,
                    destination = CardDestination.Hand,
                    count = 1,
                    actions = [new APlayThenExhaust()],
                    omitFromTooltips = true
                },
                new ACardCost
                {
                    origin = CardDestination.Hand,
                    destination = CardDestination.Hand,
                    count = 1,
                    actions = [new APlayThenExhaust()],
                    omitFromTooltips = true
                }
            ];
        return
        [
            new AAttack
            {
                damage = GetDmg(s, 2)
            },
            new ACardCost
            {
                origin = CardDestination.Hand,
                destination = CardDestination.Hand,
                count = 1,
                actions = [new APlayThenExhaust()],
                omitFromTooltips = true
            }
        ];
    }
    
    public override CardData GetData(State state)
    {
        return new CardData
        {
            cost = upgrade == Upgrade.A ? 2 : 3,
            exhaust = upgrade == Upgrade.B,
            description = ModEntry.Instance.Localizations.Localize(["card", "CardThrow", upgrade == Upgrade.B ? "descB" : "desc"], new { damage = GetDmg(state, 2) })
        };
    }
}