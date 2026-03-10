using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using Carter.Actions;
using Carter.External;

namespace Carter.Cards;

public class TrickDraw : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "TrickDraw", "name"]).Localize,
            Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Card/trick_draw.png")).Sprite
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        return [
            new ADrawCard {
                count = upgrade == Upgrade.A ? 4 : 2
            },
            new AStatus {
                status = Status.drawNextTurn,
                statusAmount = upgrade == Upgrade.B ? 3 : 1,
                targetPlayer = true
            }
        ];
    }
    
    public override CardData GetData(State state)
    {
        return new CardData
        {
            cost = 1
        };
    }

}