using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Carter.Actions;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Cards;

public class Glide : Card, IRegisterable
{


    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard(new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = ModEntry.Instance.CarterDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Glide", "name"]).Localize,
            // Art = ModEntry.Instance.card...
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> res = [
            new AStatus {
                targetPlayer = true,
                statusAmount = 1,
                status = Status.evade
            },
            new ACardCost {
                origin = CardDestination.Deck,
                destination = CardDestination.Discard,
                count = upgrade == Upgrade.B ? 2 : 1,
                actions = []
            },
            new ADrawCard
            {
                count = upgrade == Upgrade.A ? 2 : 1
            }];
        return res;
    }
    
    public override CardData GetData(State state)
    {
        return new CardData
        {
            cost = 1,
            description = ModEntry.Instance.Localizations.Localize(["card", "Glide", upgrade == Upgrade.B ? "descB" : "desc"], new { drw = upgrade == Upgrade.A ? 2 : 1 })
        };
    }
}