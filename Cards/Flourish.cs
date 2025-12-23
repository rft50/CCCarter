using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Cards;

public class Flourish : Card, IRegisterable, IHasCustomCardTraits
{


    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.Content.Cards.RegisterCard(new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = ModEntry.Instance.CarterDeck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Flourish", "name"]).Localize,
            // Art = ModEntry.Instance.card...
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        return [
            new AAttack {
                damage = GetDmg(s, upgrade == Upgrade.None ? 1 : 2),
                targetPlayer = true,
                stunEnemy = upgrade == Upgrade.B
            },
            new AMove {
                dir = 1,
                targetPlayer = true
            },
            new AAttack {
                damage = GetDmg(s, upgrade == Upgrade.B ? 2 : 1),
                targetPlayer = true,
                stunEnemy = true
            },
        ];
    }
    
    public override CardData GetData(State state)
    {
        return new CardData
        {
            cost = upgrade == Upgrade.B ? 3 : 2,
            flippable = true
        };
    }
    public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state)
        => new HashSet<ICardTraitEntry> { ModEntry.Instance.Lighten };
}