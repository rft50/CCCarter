using Carter.Features;

namespace Carter.Actions;

public class ATryUnexhaustHatTrick : CardAction
{
    public int uuid;
    public int count;

    public override void Begin(G g, State s, Combat c)
    {
        var card = s.FindCard(uuid);
        if (card != null && count == HatTrickManager.GetHatTrickLimitForUpgrade(card.upgrade) && c.exhausted.Contains(card))
        {
            s.RemoveCardFromWhereverItIs(uuid);
            c.QueueImmediate(new AAddCard
            {
                card = card,
                destination = CardDestination.Hand
            });
        }
    }
}