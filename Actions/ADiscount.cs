namespace Carter.Actions;

public class ADiscount : CardAction
{
    public override void Begin(G g, State s, Combat c)
    {
        if (selectedCard != null)
            selectedCard.discount--;
    }
}