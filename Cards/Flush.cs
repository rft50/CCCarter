using System.Collections.Generic;
using System.Reflection;
using Carter.Actions;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Cards;

public class Flush : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Flush", "name"]).Localize,
            Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Card/flush.png")).Sprite
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        var actions = new List<CardAction>();
        if (upgrade == Upgrade.A)
            actions.Add(new ADrawCard { count = 1 });
        actions.Add(new ACardCost
        {
            actions = [
                new ADrawManyOfColor { count = upgrade == Upgrade.B ? 5 : 3 }
            ]
        });

        return actions;
    }
    
    public override CardData GetData(State state)
    {
        return new CardData
        {
            cost = 1,
            description = ModEntry.Instance.Localizations.Localize(["card", "Flush", upgrade == Upgrade.A ? "descA" : "desc"], new { drw = upgrade == Upgrade.B ? 5 : 3 })
        };
    }
}