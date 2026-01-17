namespace Carter.Actions;

public class AMoveToBottomOfDraw : CardAction
{
    public override void Begin(G g, State s, Combat c)
    {
        if (selectedCard == null) return;
        
        s.RemoveCardFromWhereverItIs(selectedCard.uuid);
        s.deck.Insert(0, selectedCard);
    }
}