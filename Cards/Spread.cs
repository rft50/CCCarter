using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Carter.Actions;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Cards;

public class Spread : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Spread", "name"]).Localize,
            // Art = ModEntry.Instance.card...
        });
    }

    public int GetSpreadTotal(State s)
    {
        int num = 0;
        if (s.route is Combat combat)
        {
            num = combat.hand.Count - 1;
        }

        if (upgrade == Upgrade.A)
        {
            num++;
        }

        return num;
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> res = [];
        if (upgrade == Upgrade.A)
        {
            res.Add(new ADrawCard
            {
                count = 1
            });
        }
        res.AddRange([
            new AVariableHint
            {
                hand = true,
                handAmount = GetSpreadTotal(s)
            },
            new ADrawCard
            {
                count = GetSpreadTotal(s),
                xHint = 1
            }
        ]);
        return res;
    }
    
    public override CardData GetData(State state)
    {
        return new CardData
        {
            cost = upgrade == Upgrade.B ? 0 : 1,
            exhaust = upgrade == Upgrade.B
        };
    }
}