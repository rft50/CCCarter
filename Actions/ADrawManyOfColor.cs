namespace Carter.Actions;

public class ADrawManyOfColor : CardAction
{
    public int count;
    
    public override void Begin(G g, State s, Combat c)
    {
        var card = selectedCard;
        if (card == null) return;
        c.QueueImmediate(new ASearchForColor
        {
            uuidToIgnore = card.uuid,
            amount = count,
            deck = DB.cardMetas[card.Key()].deck,
            from = CardDestination.Deck
        });
    }
}