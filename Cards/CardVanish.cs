using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;
using Carter.Actions;
using Carter.Features;

namespace Carter.Cards;

public class CardVanish : Card, IRegisterable
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
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "CardVanish", "name"]).Localize,
            Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Card/card_vanish.png")).Sprite
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        return [
            ModEntry.Instance.KokoroApiV2.ActionCosts.MakeCostAction(
                ModEntry.Instance.KokoroApiV2.ActionCosts.MakeResourceCost(upgrade == Upgrade.B ? new ExhaustResource() : new DiscardResource(), 1),
                new AStatus
                {
                    status = Status.tempShield,
                    statusAmount = upgrade == Upgrade.A ? 2 : 1,
                    targetPlayer = true
                }
            ).AsCardAction
        ];
    }
    
    public override CardData GetData(State state)
    {
        return new CardData
        {
            cost = 0
        };
    }

}