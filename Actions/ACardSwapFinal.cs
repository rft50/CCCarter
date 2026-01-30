using Nickel;
using System;
using System.Collections.Generic;
using Carter.Features;

namespace Carter.Actions;

public class ACardSwapFinal : CardAction
{
    public CardBrowse.Source source;
    public CardBrowse.Source destination;
    public Card? selectedCardOther = null;

    public override void Begin(G g, State s, Combat c)
    {
        if (selectedCard is Card card2)
        {
            if (selectedCardOther is Card card1)
            {
                timer *= 0.5;
                doSwap(g, s, c, source, card1, card2);
                doSwap(g, s, c, destination, card2, card1);
            }
            else
            {
                doSwapHalf(g, s, c, source, card2);
            }
        }
        else
        {
            if (selectedCardOther is Card card1)
            {
                doSwapHalf(g, s, c, destination, card1);
            }
        }
    }

    private void doSwap(G g, State s, Combat c, CardBrowse.Source place, Card cardFromPlace, Card cardToPlace)
    {
        switch (place)
        {
            case CardBrowse.Source.DrawPile:
                s.deck[s.deck.IndexOf(cardFromPlace)] = cardToPlace;
                break;
            case CardBrowse.Source.Hand:
                c.hand[c.hand.IndexOf(cardFromPlace)] = cardToPlace;
                DrawDuringTurnManager.TryCardDrawAnnouncement(s, c);
                cardToPlace.OnDraw(s, c);
                foreach (var enumerateAllArtifact in s.EnumerateAllArtifacts())
                    enumerateAllArtifact.OnDrawCard(s, c, 1);
                break;
            case CardBrowse.Source.DiscardPile:
                cardToPlace.OnDiscard(s, c);
                c.SendCardToDiscard(s, cardToPlace);
                c.discard.Remove(cardToPlace);
                c.discard[c.discard.IndexOf(cardFromPlace)] = cardToPlace;
                break;
        }
    }

    private void doSwapHalf(G g, State s, Combat c, CardBrowse.Source place, Card cardToPlace)
    {
        s.RemoveCardFromWhereverItIs(cardToPlace.uuid);
        switch (place)
        {
            case CardBrowse.Source.DrawPile:
                s.deck.Add(cardToPlace);
                break;
            case CardBrowse.Source.Hand:
                c.hand.Add(cardToPlace);
                DrawDuringTurnManager.TryCardDrawAnnouncement(s, c);
                cardToPlace.OnDraw(s, c);
                foreach (var enumerateAllArtifact in s.EnumerateAllArtifacts())
                    enumerateAllArtifact.OnDrawCard(s, c, 1);
                break;
            case CardBrowse.Source.DiscardPile:
                cardToPlace.OnDiscard(s, c);
                c.SendCardToDiscard(s, cardToPlace);
                break;
        }
    }
}
