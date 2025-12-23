using Nickel;
using System;
using System.Collections.Generic;
namespace Carter.Actions;

public class ACardCost : CardAction
{
    public static Spr SprExhaust;
    public static Spr SprDiscard;
    public static Spr SprExhaustOff;
    public static Spr SprDiscardOff;
    public int count = 1;
    public enum Mode {Exhaust, Discard};

    public Mode mode = Mode.Discard;

    public List<CardAction> actions = new List<CardAction>();

    public override Route? BeginWithRoute(G g, State s, Combat c)
    {
        CardAction selectedAction = mode == Mode.Exhaust ? new AExhaustOtherCard() : new ADiscard(); // to change
        if (count <= 1)
        {
            actions.Insert(0, selectedAction);
        }
        else
        {
            List<CardAction> newActions = new List<CardAction>(actions);
            actions = [
                selectedAction,
                new ACardCost() {
                    count = count - 1,
                    mode = mode,
                    actions = newActions
                }
            ];    
        }
        CardBrowse cardBrowse = new CardBrowse
        {
            mode = CardBrowse.Mode.Browse,
            browseSource = CardBrowse.Source.Hand,
            browseAction = new AListActions { actions = actions },
            allowCancel = false
        };
        c.Queue(new ADelay
        {
            time = 0.0,
            timer = 0.0
        });
        if (cardBrowse.GetCardList(g).Count == 0)
        {
            timer = 0.0;
            return null;
        }

        return cardBrowse;
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        return
        [
            /*
             * GlossaryTooltip allows you to add arbitrary tooltips.
             * The constructor calls for a key, which is used for preventing duplicates on things with many, such as cards.
             * Icon is the sprite next to the title.
             * IconColor exists to tint the sprite, but is not used here.
             * Title is the text placed on top, next to the Icon.
             * TitleColor tints the title's text, otherwise it will be the default color.
             * Description is the body of the tooltip.
             * In addition, there is IsWideIcon (for 18 px icons), FlipIconX, and FlipIconY (for flipping the sprite)
             */
            new GlossaryTooltip($"ACardCost")
            {
                Icon = mode == Mode.Exhaust ? SprExhaust : SprDiscard,
                Title = ModEntry.Instance.Localizations.Localize(["action", "ACardCost", "name"]),
                TitleColor = Colors.card,
                Description = ModEntry.Instance.Localizations.Localize(["action", "ACardCost", "desc"], new { mode = mode, cnt = count })
            }
        ];
    }

    public override Icon? GetIcon(State s)
    {
        return new Icon
        {
            path = mode == Mode.Exhaust ? SprExhaust : SprDiscard,
            number = count == 1 ? null : count,
            // number = null,
            color = Colors.textMain
        };
    }
}
