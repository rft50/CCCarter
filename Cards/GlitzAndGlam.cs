using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using Carter.Actions;
using static CardBrowse;

namespace Carter.Cards;

public class GlitzAndGlam : Card, IRegisterable
{

    public int? value = null; // cause can't access upgrade at init, and cause B can reduce it to negatives if played without being explicitly drawn
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
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "GlitzAndGlam", "name"]).Localize,
            // Art = ModEntry.Instance.card...
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        if (value is null)
        {
            value = upgrade == Upgrade.A ? 1 : 0; // cause can't access upgrade at init
        }
        return [
            new AAttack {
                damage = GetDmg(s, (int)value)
            },
            new AStatus {
                statusAmount = (int)value,
                targetPlayer = true,
                status = Status.shield
            }
        ];
    }
    public int OnDrawAdd()
    {
        return 1;
    }
    public int OnPlaySet()
    {
        if (value is null)
        {
            value = upgrade == Upgrade.A ? 1 : 0; // cause can't access upgrade at init
        }
        return upgrade == Upgrade.B ? (int)value - 1 : 0;
    }
    public override void OnExitCombat(State s, Combat c)
    {
        value = 3;
    }
    public override void OnDraw(State s, Combat c)
    {
        value += OnDrawAdd();
    }
    public override void AfterWasPlayed(State state, Combat c)
    {
        value = OnPlaySet();
    }
    public override CardData GetData(State state)
    {
        if (value is null)
        {
            value = upgrade == Upgrade.A ? 1 : 0; // cause can't access upgrade at init
        }
        return new CardData
        {
            cost = 1,
            description = upgrade == Upgrade.B ? ModEntry.Instance.Localizations.Localize(["card", "GlitzAndGlam", "descB"], new { cur = value, drw = OnDrawAdd() }) :
                ModEntry.Instance.Localizations.Localize(["card", "GlitzAndGlam", "desc"], new { cur = value, drw = OnDrawAdd(), bse = OnPlaySet() })
        };
    }

}