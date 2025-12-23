using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using Carter.Actions;

namespace Carter.Cards;

public class CardSharp : Card, IRegisterable, IHasCustomCardTraits
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
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "CardSharp", "name"]).Localize,
            // Art = ModEntry.Instance.card...
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> res = [
            new AAttack {
                damage = GetDmg(s, upgrade == Upgrade.B ? 3 : 2)
            }
        ];
        if (upgrade == Upgrade.A)
        {
            res.Add(new AStatus
            {
                status = Status.drawNextTurn,
                statusAmount = 1,
                targetPlayer = true
            });
        }
        return res;
    }

    public override CardData GetData(State state)
    {
        return new CardData
        {
            cost = 1,
            recycle = true
        };
    }
    public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state)
        => new HashSet<ICardTraitEntry> { ModEntry.Instance.KokoroApiV2.Fleeting.Trait };

}