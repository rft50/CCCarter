using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using Carter.Actions;

namespace Carter.Cards;

public class SleightOfHand : Card, IRegisterable
{
    public static Spr top;
    public static Spr bottom;
    public static Spr regular;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        top = helper.Content.Sprites
            .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Card/sleight_of_hand_upper.png")).Sprite;
        bottom = helper.Content.Sprites
            .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Card/sleight_of_hand_lower.png")).Sprite;
        regular = helper.Content.Sprites
            .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Card/sleight_of_hand.png")).Sprite;
        helper.Content.Cards.RegisterCard(new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = ModEntry.Instance.CarterDeck.Deck,
                rarity = Rarity.rare,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "SleightOfHand", "name"]).Localize,
            Art = regular
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
         List<CardAction> res = [
        ];
        if (upgrade == Upgrade.A)
        {
            res.Add(new ADrawCard {
                count = 1,
                disabled = flipped
            });
        }
        res.Add(
            new ACardSwap
            {
                source = CardBrowse.Source.Hand,
                destination = CardBrowse.Source.DrawPile,
                disabled = flipped && upgrade != Upgrade.B
            });
        if (upgrade != Upgrade.B)
        {
            res.Add(new ADummyAction());
        }
        if (upgrade == Upgrade.A)
        {
            res.Add(new ADrawCard {
                count = 1,
                disabled = !flipped
            });
        }
        res.Add(
            new ACardSwap
            {
                source = CardBrowse.Source.Hand,
                destination = CardBrowse.Source.DiscardPile,
                disabled = !flipped && upgrade != Upgrade.B
            });
        return res;
    }
    
    public override CardData GetData(State state)
    {
        return new CardData
        {
            cost = 0,
            exhaust = true,
            floppable = upgrade != Upgrade.B,
            art = upgrade != Upgrade.B ? (flipped ? bottom : top) : regular,
            artTint = upgrade != Upgrade.B ? "FFFFFF" : null
        };
    }
}