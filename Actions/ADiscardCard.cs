using Nickel;
using System;
using System.Collections.Generic;
namespace Carter.Actions;

public class ADiscardCard : CardAction
{
    public static Spr Spr;

    public override void Begin(G g, State s, Combat c)
    {
        if (selectedCard == null)
        {
            // must have selected the card to discard
            return;
        }
        s.RemoveCardFromWhereverItIs(selectedCard.uuid);
        c.SendCardToDiscard(s, selectedCard);
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
            new GlossaryTooltip($"AExhaustCard")
            {
                Icon = Spr,
                Title = ModEntry.Instance.Localizations.Localize(["action", "ADiscardCard", "name"]),
                TitleColor = Colors.card,
                Description = ModEntry.Instance.Localizations.Localize(["action", "ADiscardCard", "desc"])
            }
        ];
    }

    public override Icon? GetIcon(State s)
    {
        return new Icon
        {
            path = Spr,
            // number = count == 1 ? null : count,
            number = null,
            color = Colors.textMain
        };
    }
}
