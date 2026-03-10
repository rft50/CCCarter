using System.Collections.Generic;
using System.Reflection;
using Carter.Actions;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Cards;

public class Cardistry : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Cardistry", "name"]).Localize,
            Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Card/cardistry.png")).Sprite
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        return
        [
            new AStatus
            {
                status = ModEntry.Instance.CardistryStatus.Status,
                statusAmount = upgrade == Upgrade.B ? 2 : 1,
                targetPlayer = true
            },
            new AStatus
            {
                status = Status.drawNextTurn,
                statusAmount = upgrade == Upgrade.B ? 2 : 1,
                targetPlayer = true
            }
        ];
    }
    
    public override CardData GetData(State state)
    {
        return new CardData
        {
            cost = upgrade switch
            {
                Upgrade.A => 1,
                Upgrade.B => 3,
                _ => 2
            },
            exhaust = true
        };
    }
}