using Nickel;
using System;
using System.Collections.Generic;
namespace Carter.Actions;

public class ACardSwapInternal : CardAction
{
    public CardBrowse.Source source;
    public CardBrowse.Source destination;
    public int? sourceUuid;

    public override Route? BeginWithRoute(G g, State s, Combat c)
    {
        /*if (selectedCard == null)
        {
            // must have selected the card to swap with
            return null;
        }*/
        CardBrowse cardBrowse = new CardBrowse
        {
            mode = CardBrowse.Mode.Browse,
            browseSource = destination,
            browseAction = new ACardSwapFinal {
                source = source,
                destination = destination,
                selectedCardOther = selectedCard
            },
            allowCancel = false,
            filterUUID = sourceUuid
        };
        c.Queue(new ADelay
        {
            time = 0.0,
            timer = 0.0
        });
        if (cardBrowse.GetCardList(g).Count == 0)
        {
            timer = 0.0;
            new ACardSwapFinal
            {
                source = source,
                destination = destination,
                selectedCardOther = selectedCard,
            }.Begin(g, s, c);
            return null;
        }

        return cardBrowse;
    }
}
