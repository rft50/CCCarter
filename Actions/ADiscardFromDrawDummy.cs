using System.Collections.Generic;
using Nickel;

namespace Carter.Actions;

public class ADiscardFromDrawDummy : CardAction
{
    public static Spr Spr;

    public int count;

    public override Icon? GetIcon(State s) => new Icon(Spr, count, Colors.textMain);

    public override List<Tooltip> GetTooltips(State s)
    {
        return
        [
            new GlossaryTooltip("ADiscardFromDraw")
            {
                Icon = Spr,
                Title = ModEntry.Instance.Localizations.Localize(["action", "ADiscardFromDraw", "name"]),
                TitleColor = Colors.card,
                Description = ModEntry.Instance.Localizations.Localize(["action", "ADiscardFromDraw", "desc"], new { cnt = count })
            }
        ];
    }

    public static CardAction Spoof(ACardCost cost)
    {
        return ModEntry.Instance.KokoroApiV2.SpoofedActions.MakeAction(
            new ADiscardFromDrawDummy
            {
                count = cost.count
            },
            cost
        ).AsCardAction;
    }
}