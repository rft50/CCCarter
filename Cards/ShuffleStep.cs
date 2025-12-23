using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Cards;

public class ShuffleStep : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "ShuffleStep", "name"]).Localize,
            // Art = ModEntry.Instance.card...
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        return [
            new AMove {
                dir = upgrade == Upgrade.B ? -2 : -1,
                targetPlayer = true
            },
            new ADrawCard {
                count = upgrade == Upgrade.A ? 3 : 2
            }
        ];
    }
    
    public override CardData GetData(State state)
    {
        return new CardData
        {
            cost = 1,
            flippable = true
        };
    }
}