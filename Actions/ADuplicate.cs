namespace Carter.Actions;

public class ADuplicateTopdeck : CardAction 
{
    public bool makeTemp = true;

    public override void Begin(G g, State s, Combat c)
    {
        if (s.deck.Count == 0) return;
        var card = s.deck[^1].CopyWithNewId();
        if (makeTemp)
            ModEntry.Instance.Helper.Content.Cards.SetCardTraitOverride(s, card, ModEntry.Instance.Helper.Content.Cards.TemporaryCardTrait, true, true);
        c.QueueImmediate(new AAddCard
        {
            card = card,
            destination = CardDestination.Hand
        });
    }
}