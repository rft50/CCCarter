using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using Carter.Actions;

namespace Carter.Cards;

public class CardVanish : Card, IRegisterable
{


    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard(new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = ModEntry.Instance.CarterDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "CardVanish", "name"]).Localize,
            // Art = ModEntry.Instance.card...
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        return [
            new ACardCost {
                mode = upgrade == Upgrade.B ? ACardCost.Mode.Exhaust : ACardCost.Mode.Discard,
                actions = [
                    new AStatus {
                        status = Status.tempShield,
                        statusAmount = upgrade == Upgrade.A ? 2 : 1,
                        targetPlayer = true
                    }
                ]
            },
            new AStatus {
                status = Status.tempShield,
                statusAmount = upgrade == Upgrade.A ? 2 : 1,
                targetPlayer = true,
                disabled = true
            }
        ];
    }
    
    public override CardData GetData(State state)
    {
        return new CardData
        {
            cost = 0
        };
    }

}