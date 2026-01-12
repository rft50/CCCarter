using Nickel;
using System;
using System.Collections.Generic;
using Carter.External;
using Carter.Features;

namespace Carter.Actions;

public class ACardCost : CardAction
{
    public int count = 1;

    public bool optional = false;

    public CardDestination origin = CardDestination.Hand;

    public CardDestination destination = CardDestination.Discard;

    public List<CardAction> actions = [];

    public override Route? BeginWithRoute(G g, State s, Combat c)
    {
        CardAction selectedAction = new ACardCostPayment
        {
            mode = destination
        };
        actions.Insert(0, selectedAction);
        CardBrowse cardBrowse = new CardBrowse
        {
            mode = CardBrowse.Mode.Browse,
            browseSource = Source(origin),
            browseAction = new AMultiBrowseListActions { actions = actions },
            allowCancel = optional
        };
        if (cardBrowse.GetCardList(g).Count == 0)
        {
            timer = 0.0;
            return null;
        }
        count = Math.Min(count, cardBrowse.GetCardList(g).Count);
        
        var multiBrowseRoute = ModEntry.Instance.KokoroApiV2.MultiCardBrowse.MakeRoute(cardBrowse);
        multiBrowseRoute.MaxSelected = count;
        multiBrowseRoute.MinSelected = optional ? 0 : count;
        
        return multiBrowseRoute.AsRoute;
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        return
        [
            new GlossaryTooltip($"ACardCost")
            {
                Icon = destination == CardDestination.Exhaust ? CostManager.SprExhaust : CostManager.SprDiscard,
                Title = ModEntry.Instance.Localizations.Localize(["action", "ACardCost", "name"]),
                TitleColor = Colors.card,
                Description = ModEntry.Instance.Localizations.Localize(["action", "ACardCost", "desc"], new { mode = destination, cnt = count })
            }
        ];
    }
    public override Icon? GetIcon(State s)
    {
        return new Icon
        {
            path = destination == CardDestination.Exhaust ? CostManager.SprExhaust : CostManager.SprDiscard,
            number = count == 1 ? null : count,
            // number = null,
            color = Colors.textMain
        };
    }

    public static CardBrowse.Source Source(CardDestination mode)
    {
        return mode switch
        {
            CardDestination.Discard => CardBrowse.Source.DiscardPile,
            CardDestination.Exhaust => CardBrowse.Source.ExhaustPile,
            CardDestination.Hand => CardBrowse.Source.Hand,
            CardDestination.Deck => CardBrowse.Source.DrawPile,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }

}

public class ACardCostPayment : CardAction
{
    public CardDestination mode;
    public override void Begin(G g, State s, Combat c)
    {
        timer = 0.0;
        if (selectedCard == null) return;
        if (mode != CardDestination.Hand)
        {
            s.RemoveCardFromWhereverItIs(selectedCard.uuid);
        }

        if (mode == CardDestination.Exhaust)
        {
            c.SendCardToExhaust(s, selectedCard);
        }
        else if (mode == CardDestination.Discard)
        {
            c.SendCardToDiscard(s, selectedCard);
        }
        else if (mode == CardDestination.Deck)
        {
            s.SendCardToDeck(selectedCard);
        }
    }
}