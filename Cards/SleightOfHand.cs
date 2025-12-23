using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using Carter.Actions;

namespace Carter.Cards;

public class SleightOfHand : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "SleightOfHand", "name"]).Localize,
            // Art = ModEntry.Instance.card...
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
         List<CardAction> res = [
        ];
        if (upgrade == Upgrade.A)
        {
            res.Add(new ADrawCard {
                count = 1,
                disabled = flipped
            });
        }
        res.Add(
            new ACardSwap
            {
                source = CardBrowse.Source.Hand,
                destination = CardBrowse.Source.DrawPile,
                disabled = flipped && upgrade != Upgrade.B
            });
        if (upgrade != Upgrade.B)
        {
            res.Add(new ADummyAction());
        }
        if (upgrade == Upgrade.A)
        {
            res.Add(new ADrawCard {
                count = 1,
                disabled = !flipped
            });
        }
        res.Add(
            new ACardSwap
            {
                source = CardBrowse.Source.Hand,
                destination = CardBrowse.Source.DiscardPile,
                disabled = !flipped && upgrade != Upgrade.B
            });
        return res;
    }
    
    public override CardData GetData(State state)
    {
        return new CardData
        {
            cost = 0,
            exhaust = true,
            floppable = upgrade != Upgrade.B,
            art = upgrade != Upgrade.B ? (flipped ? ModEntry.Instance.cardBottom : ModEntry.Instance.cardTop) : null
        };
    }
}