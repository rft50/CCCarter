using Nickel;
using System;
using System.Collections.Generic;
namespace Carter.Actions;

public class AMultiBrowseListActions : CardAction
{
    public List<CardAction> actions = new List<CardAction>();

    public override void Begin(G g, State s, Combat c)
    {
        if (ModEntry.Instance.KokoroApiV2.MultiCardBrowse.GetSelectedCards(this) is not { } selectedCards)
            return;
        
        var toQueue = new List<CardAction>();

        foreach (var card in selectedCards)
        {
            foreach (var cardAction in actions)
            {
                var action = Mutil.DeepCopy(cardAction);
                action.selectedCard = card;
                toQueue.Add(action);
            }
        }

        c.QueueImmediate(toQueue);
    }
}
