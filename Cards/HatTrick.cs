using System.Collections.Generic;
using System.Reflection;
using Carter.Actions;
using Carter.Features;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Cards;

public class HatTrick : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "HatTrick", "name"]).Localize,
            // Art = ModEntry.Instance.card...
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        return
        [
            new AAttack
            {
                damage = GetDmg(s, upgrade == Upgrade.B ? 2 : 1)
            }
        ];
    }
    
    public override CardData GetData(State state)
    {
        return new CardData
        {
            cost = 0,
            description = ModEntry.Instance.Localizations.Localize(["card", "HatTrick", "desc"], new
            {
                damage = GetDmg(state, upgrade == Upgrade.B ? 2 : 1),
                count = HatTrickManager.GetCardPlayCount(state),
                limit = HatTrickManager.GetHatTrickLimitForUpgrade(upgrade)
            }),
            exhaust = true
        };
    }
}