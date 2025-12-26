namespace Carter.Actions;

public class APlayThenExhaust : CardAction
{
    public override void Begin(G g, State s, Combat c)
    {
        if (selectedCard == null) return;
        c.QueueImmediate([
            ModEntry.Instance.KokoroApiV2.PlayCardsFromAnywhere.MakeAction(selectedCard!).AsCardAction,
            new AExhaustCardAnywhere { selectedCard = selectedCard }
        ]);
    }
}

public class AExhaustCardAnywhere : CardAction
{
    public override void Begin(G g, State s, Combat c)
    {
        s.RemoveCardFromWhereverItIs(selectedCard!.uuid);
        c.SendCardToExhaust(s, selectedCard);
    }
}