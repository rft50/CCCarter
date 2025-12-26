using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using Carter.Actions;
using Carter.External;

namespace Carter.Cards;

public class Cantrip : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Cantrip", "name"]).Localize,
            // Art = ModEntry.Instance.card...
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> res = [];
        if (upgrade != Upgrade.B)
        {
            res.Add(new ADrawCard
            {
                count = 1
            });
        }
        res.AddRange([
            new AStatus {
                status = ModEntry.Instance.StackStatus.Status,
                statusAmount = upgrade switch
                {
                    Upgrade.B => 3,
                    Upgrade.A => 2,
                    _ => 1
                },
                targetPlayer = true
            }
        ]);
        return res;
    }
    
    public override CardData GetData(State state)
    {
        return new CardData
        {
            cost = 0,
            exhaust = true
        };
    }

}