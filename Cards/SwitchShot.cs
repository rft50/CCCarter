using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using Carter.Actions;

namespace Carter.Cards;

public class SwitchShot : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "SwitchShot", "name"]).Localize,
            Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Card/switch_shot.png")).Sprite
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
         List<CardAction> res = [
            new AAttack {
                damage = GetDmg(s, upgrade == Upgrade.A ? 2 : 1)
            },
            new ACardSwap {
                source = CardBrowse.Source.DrawPile,
                destination = CardBrowse.Source.DiscardPile
            }
        ];
        if (upgrade == Upgrade.B)
        {
            res.Add(new ADrawCard { count = 1 });
        }
        return res;
    }
    
    public override CardData GetData(State state)
    {
        return new CardData
        {
            cost = 1
        };
    }
}