using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Carter.Actions;
using Carter.External;
using Carter.Features;
using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;

namespace Carter.Artifacts.Duos;

public interface IDuoArtifact
{
    public static abstract void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi);
}

public class CarterDizzyDuo : Artifact, IDuoArtifact, IDrawDuringTurnHook
{
    public int count = 0;
    
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.Content.Artifacts.RegisterArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/Duo/card_sleeve.png"))
                .Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Dizzy", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Dizzy", "desc"]).Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!,
            [ModEntry.Instance.CarterDeck.Deck, Deck.dizzy]);
    }

    public override List<Tooltip> GetExtraTooltips() =>
    [
        new TTGlossary("status.tempShield", 1)
    ];

    public override int? GetDisplayNumber(State s)
    {
        return count;
    }

    public void OnDrawDuringTurn(State s, Combat c)
    {
        count++;

        if (count >= 3)
        {
            count -= 3;
            c.Queue(new AStatus
            {
                status = Status.tempShield,
                statusAmount = 1,
                targetPlayer = true,
                artifactPulse = Key()
            });
        }
    }
}

public class CarterRiggsDuo : Artifact, IDuoArtifact
{
    public bool riggs = false;
    public bool carter = false;
    public bool activated = false;

    public static Spr[] sprites = new Spr[4];
    
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        sprites[0] = helper.Content.Sprites
            .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/Duo/gold_card_0.png"))
            .Sprite;
        sprites[1] = helper.Content.Sprites
            .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/Duo/gold_card_1.png"))
            .Sprite;
        sprites[2] = helper.Content.Sprites
            .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/Duo/gold_card_2.png"))
            .Sprite;
        sprites[3] = helper.Content.Sprites
            .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/Duo/gold_card_3.png"))
            .Sprite;
        helper.Content.Artifacts.RegisterArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck
            },
            Sprite = sprites[3],
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Riggs", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Riggs", "desc"]).Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!,
            [ModEntry.Instance.CarterDeck.Deck, Deck.riggs]);
    }

    public override List<Tooltip> GetExtraTooltips() =>
    [
        new TTGlossary("status.drawNextTurn", 1)
    ];

    public int CurrentState()
    {
        return (riggs ? 1 : 0) + (carter ? 2 : 0);
    }

    public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition,
        int handCount)
    {
        if (deck == Deck.riggs)
            riggs = true;
        if (deck == ModEntry.Instance.CarterDeck.Deck)
            carter = true;
        if (riggs && carter && !activated)
        {
            activated = true;
            combat.Queue(new AStatus
            {
                status = Status.drawNextTurn,
                statusAmount = 1,
                targetPlayer = true,
                artifactPulse = Key()
            });
        }
    }

    public override Spr GetSprite()
    {
        return sprites[CurrentState()];
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        riggs = carter = activated = false;
    }
    
    public override void OnCombatEnd(State state)
    {
        riggs = carter = true;
    }
}

public class CarterPeriDuo : Artifact, IDuoArtifact
{
    public int count;
    
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.Content.Artifacts.RegisterArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/Duo/gun_oil.png"))
                .Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Peri", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Peri", "desc"]).Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!,
            [ModEntry.Instance.CarterDeck.Deck, Deck.peri]);
    }

    public override int? GetDisplayNumber(State s)
    {
        return count;
    }

    public override void OnPlayerAttack(State state, Combat combat)
    {
        count++;
        if (count >= 7)
        {
            count -= 7;
            combat.Queue(new ADrawCard
            {
                count = 1,
                artifactPulse = Key()
            });
        }
    }
}

public class CarterIsaacDuo : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.Content.Artifacts.RegisterArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/Duo/torn_card.png"))
                .Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Isaac", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Isaac", "desc"]).Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!,
            [ModEntry.Instance.CarterDeck.Deck, Deck.goat]);
        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.DestroyDroneAt)),
            postfix: new HarmonyMethod(typeof(CarterIsaacDuo), nameof(Combat_DestroyDroneAt_Postfix))
        );
    }

    private static void Combat_DestroyDroneAt_Postfix(Combat __instance, State s, bool playerDidIt)
    {
        if (playerDidIt) return;
        
        var artifact = s.EnumerateAllArtifacts().OfType<CarterIsaacDuo>().FirstOrDefault((CarterIsaacDuo?)null);
        if (artifact == null) return;
        
        __instance.QueueImmediate(new ADrawCard
        {
            count = 1,
            artifactPulse = artifact.Key()
        });
    }
}

public class CarterDrakeDuo : Artifact, IDuoArtifact
{
    public static Spr on;
    public static Spr off;
    
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        on = helper.Content.Sprites
            .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/Duo/ace_of_diamonds_on.png"))
            .Sprite;
        off = helper.Content.Sprites
            .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/Duo/ace_of_diamonds.png"))
            .Sprite;
        helper.Content.Artifacts.RegisterArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck
            },
            Sprite = off,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Drake", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Drake", "desc"]).Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!,
            [ModEntry.Instance.CarterDeck.Deck, Deck.eunice]);
        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(ADrawCard), nameof(ADrawCard.Begin)),
            prefix: new HarmonyMethod(typeof(CarterDrakeDuo), nameof(ADrawCard_Begin_Prefix))
        );
    }

    private static void ADrawCard_Begin_Prefix(ADrawCard __instance, State s)
    {
        if (s.ship.Get(Status.heat) < 3) return;
        
        var artifact = s.EnumerateAllArtifacts().OfType<CarterDrakeDuo>().FirstOrDefault((CarterDrakeDuo?)null);
        if (artifact == null) return;

        __instance.count++;
        artifact.Pulse();
    }

    public override Spr GetSprite()
    {
        return (MG.inst.g.state.ship.Get(Status.heat) >= 3) ? on : off;
    }
}

public class CarterMaxDuo : Artifact, IDuoArtifact
{
    public int count;
    
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.Content.Artifacts.RegisterArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/Duo/back_door.png"))
                .Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Max", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Max", "desc"]).Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!,
            [ModEntry.Instance.CarterDeck.Deck, Deck.hacker]);
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        count++;
        if (count >= 2)
        {
            count -= 2;
            combat.Queue(new ACardSelect
            {
                browseAction = new AMoveToBottomOfDraw(),
                browseSource = CardBrowse.Source.ExhaustPile,
                artifactPulse = Key()
            });
        }
    }

    public override int? GetDisplayNumber(State s)
    {
        return count;
    }
}

public class CarterBooksDuo : Artifact, IDuoArtifact
{
    public bool ready = true;

    public static Spr on;
    public static Spr off;
    
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        on = helper.Content.Sprites
            .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/Duo/sparkling_wand.png"))
            .Sprite;
        off = helper.Content.Sprites
            .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/Duo/sparkling_wand_off.png"))
            .Sprite;
        helper.Content.Artifacts.RegisterArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck
            },
            Sprite = on,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Books", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Books", "desc"]).Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!,
            [ModEntry.Instance.CarterDeck.Deck, Deck.shard]);
    }

    public override List<Tooltip>? GetExtraTooltips() =>
    [
        StatusMeta.GetTooltips(ModEntry.Instance.StackStatus.Status, 1)[0]
    ];

    public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition,
        int handCount)
    {
        if (!ready) return;
        if (card is not ShardCard) return;

        ready = false;
        combat.Queue(new AStatus
            {
                statusAmount = 1,
                status = ModEntry.Instance.StackStatus.Status,
                targetPlayer = true,
                artifactPulse = Key()
            }
        );
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        ready = true;
    }

    public override void OnCombatEnd(State state)
    {
        ready = true;
    }

    public override Spr GetSprite()
    {
        return ready ? on : off;
    }
}

public class CarterCatDuo : Artifact, IDuoArtifact
{
    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper, IDuoApi duoApi)
    {
        helper.Content.Artifacts.RegisterArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = duoApi.DuoArtifactVanillaDeck
            },
            Sprite = helper.Content.Sprites
                .RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifact/Duo/auto_shuffler.png"))
                .Sprite,
            Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Cat", "name"]).Localize,
            Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "duo", "Cat", "desc"]).Localize
        });
        duoApi.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!,
            [ModEntry.Instance.CarterDeck.Deck, Deck.colorless]);
        ModEntry.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(State), nameof(State.ShuffleDeck)),
            postfix: new HarmonyMethod(typeof(CarterCatDuo), nameof(State_ShuffleDeck_Postfix))
        );
    }

    private static void State_ShuffleDeck_Postfix(State __instance, bool isMidCombat)
    {
        if (!isMidCombat) return;
        
        var artifact = __instance.EnumerateAllArtifacts().OfType<CarterCatDuo>().FirstOrDefault((CarterCatDuo?)null);
        if (artifact == null) return;

        List<Card> buffer = [];

        for (var i = __instance.deck.Count - 1; i >= 0; i--)
        {
            if (ModEntry.Instance.Helper.Content.Cards.IsCardTraitActive(__instance, __instance.deck[i],
                    ModEntry.Instance.Helper.Content.Cards.TemporaryCardTrait))
            {
                buffer.Add(__instance.deck[i]);
                __instance.deck.RemoveAt(i);
            }
        }
        for (var i = buffer.Count - 1; i >= 0; i--) __instance.deck.Add(buffer[i]);
        if (buffer.Count > 0) artifact.Pulse();
    }
}