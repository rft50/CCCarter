using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using Carter.Actions;

namespace Carter.Cards;

public class Stack : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Stack", "name"]).Localize,
            Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Card/stack.png")).Sprite
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> res = [
            new AStatus {
                status = Status.shield,
                targetPlayer = true,
                statusAmount = upgrade == Upgrade.B ? 2 : 1
            }
        ];
        if (upgrade == Upgrade.A)
        {
            res.Add(new ADrawCard
            {
                count = 1
            });
        }
        res.Add(new AStatus
        {
            status = ModEntry.Instance.StackStatus.Status,
            statusAmount = 1,
            targetPlayer = true
        });
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