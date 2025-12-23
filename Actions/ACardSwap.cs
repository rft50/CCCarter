using Nickel;
using System;
using System.Collections.Generic;
namespace Carter.Actions;

public class ACardSwap : CardAction
{
    public static Spr Spr;
    public CardBrowse.Source source;
    public CardBrowse.Source destination;

    public override Route? BeginWithRoute(G g, State s, Combat c)
    {
        CardBrowse cardBrowse = new CardBrowse
        {
            mode = CardBrowse.Mode.Browse,
            browseSource = source,
            browseAction = new ACardSwapInternal {
                source = source,
                destination = destination
            },
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
            return new ACardSwapInternal
            {
                source = source,
                destination = destination
            }.BeginWithRoute(g, s, c);
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
            new GlossaryTooltip($"ACardSwap")
            {
                Icon = Spr,
                Title = ModEntry.Instance.Localizations.Localize(["action", "ACardSwap", "name"]),
                TitleColor = Colors.card,
                Description = ModEntry.Instance.Localizations.Localize(["action", "ACardSwap", "desc"], new { src = source, dest = destination })
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
