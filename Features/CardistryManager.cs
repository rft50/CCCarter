using Carter.Actions;

namespace Carter.Features;

public class CardistryManager
{
    public CardistryManager()
    {
        ModEntry.Instance.Helper.Events.RegisterAfterArtifactsHook("OnTurnEnd", CardistryEffect);
    }
    
    public static void CardistryEffect(State state, Combat combat)
    {
        var cardistry = state.ship.Get(ModEntry.Instance.CardistryStatus.Status);
        if (cardistry >= 1)
        {
            
            combat.Queue(new ACardCost
            {
                count = cardistry,
                optional = true,
                origin = CardDestination.Discard,
                destination = CardDestination.Deck
            });
        }
    }
}