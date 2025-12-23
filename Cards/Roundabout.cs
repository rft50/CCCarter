using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using Carter.Actions;

namespace Carter.Cards;

public class Roundabout : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Roundabout", "name"]).Localize,
            // Art = ModEntry.Instance.card...
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> res = [];
        if (upgrade == Upgrade.B)
        {
            res.Add(
                new AStatus
                {
                    status = Status.shield,
                    statusAmount = 1,
                    targetPlayer = true
                });
        }
            
        res.AddRange([
            new AStatus
            {
                status = Status.tempShield,
                statusAmount = 2,
                targetPlayer = true
            },
            new ACardSwap {
                source = CardBrowse.Source.Hand,
                destination = CardBrowse.Source.DiscardPile
            }
        ]);
        return res;
    }
    
    public override CardData GetData(State state)
    {
        return new CardData
        {
            cost = upgrade == Upgrade.A ? 1 : 2,
        };
    }
}