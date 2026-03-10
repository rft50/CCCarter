using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using Carter.Actions;
using static CardBrowse;

namespace Carter.Cards;

public class RazzleDazzle : Card, IRegisterable
{

    public int value = 3;
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
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "RazzleDazzle", "name"]).Localize,
            Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Card/razzle_dazzle.png")).Sprite
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        return [
            new AAttack {
                damage = GetDmg(s, value)
            }
        ];
    }
    public int OnDrawAdd()
    {
        return upgrade == Upgrade.B ? 2 : 1;
    }
    public int OnPlaySet()
    {
        return upgrade == Upgrade.A ? value : upgrade == Upgrade.B ? 1 : 3;
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
        return new CardData
        {
            cost = 2,
            description = upgrade != Upgrade.A ? ModEntry.Instance.Localizations.Localize(["card", "RazzleDazzle", "desc"], new { cur = value, drw = OnDrawAdd(), bse = OnPlaySet() }) :
            ModEntry.Instance.Localizations.Localize(["card", "RazzleDazzle", "descA"], new { cur = value, drw = OnDrawAdd() })
        };
    }

}