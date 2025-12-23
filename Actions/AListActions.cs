using Nickel;
using System;
using System.Collections.Generic;
namespace Carter.Actions;

public class AListActions : CardAction
{
    public List<CardAction> actions = new List<CardAction>();

    public override void Begin(G g, State s, Combat c)
    {
        foreach (CardAction action in actions)
        {
            action.selectedCard = selectedCard;
            action.Begin(g, s, c);
        }
    }
}
