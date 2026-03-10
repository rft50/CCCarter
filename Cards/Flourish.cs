using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Cards;

public class Flourish : Card, IRegisterable, IHasCustomCardTraits
{
    public static Spr left;
    public static Spr right;

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        left = helper.Content.Sprites
            .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Card/flourish_left.png")).Sprite;
        right = helper.Content.Sprites
            .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Card/flourish_right.png")).Sprite;
        helper.Content.Cards.RegisterCard(new CardConfiguration
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new CardMeta
            {
                deck = ModEntry.Instance.CarterDeck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Flourish", "name"]).Localize,
            Art = right
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c)
    {
        return [
            new AAttack {
                damage = GetDmg(s, upgrade == Upgrade.None ? 1 : 2),
                stunEnemy = upgrade == Upgrade.B
            },
            new AMove {
                dir = 1,
                targetPlayer = true
            },
            new AAttack {
                damage = GetDmg(s, upgrade == Upgrade.B ? 2 : 1),
                stunEnemy = true
            },
        ];
    }
    
    public override CardData GetData(State state)
    {
        return new CardData
        {
            cost = upgrade == Upgrade.B ? 3 : 2,
            flippable = true,
            art = flipped ? left : right
        };
    }
    public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state)
        => new HashSet<ICardTraitEntry> { ModEntry.Instance.Lighten };
}