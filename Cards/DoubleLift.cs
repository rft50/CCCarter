using System.Collections.Generic;
using System.Reflection;
using Carter.Actions;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Cards;

public class DoubleLift : Card, IRegisterable
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard(new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = ModEntry.Instance.CarterDeck.Deck,
                rarity = Rarity.rare,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "DoubleLift", "name"]).Localize,
            // Art = ModEntry.Instance.card...
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        return
        [
            new ADuplicateTopdeck
            {
                makeTemp = upgrade != Upgrade.B
            },
            new ADrawCard
            {
                count = 1
            }
        ];
    }
    
    public override CardData GetData(State state)
    {
        return new CardData
        {
            cost = upgrade == Upgrade.A ? 0 : 1,
            description = ModEntry.Instance.Localizations.Localize(["card", "DoubleLift", upgrade == Upgrade.B ? "descB" : "desc"]),
            exhaust = upgrade != Upgrade.B,
            singleUse = upgrade == Upgrade.B
        };
    }
}