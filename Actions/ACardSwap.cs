using Nickel;
using System;
using System.Collections.Generic;
namespace Carter.Actions;

public class ACardSwap : CardAction
{
    public static Spr Spr;
    public static Spr DrawDiscard;
    public static Spr HandDiscard;
    public static Spr HandDraw;
    public CardBrowse.Source source;
    public CardBrowse.Source destination;

    public override Route? BeginWithRoute(G g, State s, Combat c)
    {
        var sourceUuid = ModEntry.Instance.KokoroApiV2.ActionInfo.GetSourceCardId(this);
        CardBrowse cardBrowse = new CardBrowse
        {
            mode = CardBrowse.Mode.Browse,
            browseSource = source,
            browseAction = new ACardSwapInternal {
                source = source,
                destination = destination,
                sourceUuid = sourceUuid
            },
            allowCancel = false,
            filterUUID = sourceUuid
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
                destination = destination,
                sourceUuid = sourceUuid
            }.BeginWithRoute(g, s, c);
        }

        return cardBrowse;
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        return
        [
            new GlossaryTooltip($"ACardSwap:{source}:{destination}")
            {
                Icon = GetSprite(source, destination),
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
            path = GetSprite(source, destination),
            color = Colors.textMain
        };
    }

    public static Spr GetSprite(CardBrowse.Source source, CardBrowse.Source destination)
    {
        if (source == CardBrowse.Source.DrawPile && destination == CardBrowse.Source.DiscardPile) return DrawDiscard;
        if (source == CardBrowse.Source.DiscardPile && destination == CardBrowse.Source.DrawPile) return DrawDiscard;
        
        if (source == CardBrowse.Source.Hand && destination == CardBrowse.Source.DiscardPile) return HandDiscard;
        if (source == CardBrowse.Source.DiscardPile && destination == CardBrowse.Source.Hand) return HandDiscard;
        
        if (source == CardBrowse.Source.Hand && destination == CardBrowse.Source.DrawPile) return HandDraw;
        if (source == CardBrowse.Source.DrawPile && destination == CardBrowse.Source.Hand) return HandDraw;
        
        return Spr;
    }
}
