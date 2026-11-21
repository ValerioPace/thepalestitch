using System.Linq;
using Godot;

namespace ThePaleStitch.Character;

/// <summary>The six things a cutthroat can be good at. Values go from 1 to 5.</summary>
public enum Skill
{
    /// <summary>How little noise the feet make.</summary>
    Tread,
    /// <summary>How well the body melts into poor light.</summary>
    Shadow,
    /// <summary>Locks, latches and other people's purses.</summary>
    Fingers,
    /// <summary>Killing quickly and without a sound.</summary>
    Blade,
    /// <summary>Poisons, sleeping draughts, tainted cups.</summary>
    Venom,
    /// <summary>Climbing, and carrying what cannot walk any more.</summary>
    Sinew,
}

/// <summary>A trade: the leaning of a man who is always both thief and murderer.</summary>
public sealed record TradeDef(string Id, int[] Skills)
{
    public string NameKey => $"TRADE_{Id.ToUpperInvariant()}_NAME";
    public string DescKey => $"TRADE_{Id.ToUpperInvariant()}_DESC";
    public string PerkNameKey => $"TRADE_{Id.ToUpperInvariant()}_PERK";
    public string PerkDescKey => $"TRADE_{Id.ToUpperInvariant()}_PERK_DESC";
    public int this[Skill skill] => Skills[(int)skill];
}

/// <summary>A dye of a costume (the "skin"): colours only, no gameplay effect.</summary>
public sealed record DyeDef(string Id, Color Cloth, Color Trim)
{
    public string NameKey => $"VARIANT_{Id.ToUpperInvariant()}";
}

/// <summary>
/// Starting garb. <see cref="Visibility"/> and <see cref="Noise"/> are multipliers:
/// below 1 is better for the player.
/// </summary>
public sealed record CostumeDef(string Id, float Visibility, float Noise, int BeltSlots, DyeDef[] Dyes)
{
    public string NameKey => $"COSTUME_{Id.ToUpperInvariant()}_NAME";
    public string DescKey => $"COSTUME_{Id.ToUpperInvariant()}_DESC";
    public string GuiseKey => $"COSTUME_{Id.ToUpperInvariant()}_GUISE";
}

/// <summary>A face. Every one of them is one a mother would cross the street to avoid.</summary>
public sealed record VisageDef(string Id)
{
    public string NameKey => $"VISAGE_{Id.ToUpperInvariant()}_NAME";
    public string DescKey => $"VISAGE_{Id.ToUpperInvariant()}_DESC";
}

/// <summary>
/// Everything the character creation screen offers. Texts live in
/// res://localization/strings.csv under the keys built by the records above.
/// </summary>
public static class CharacterCatalog
{
    public const int MaxSkill = 5;

    public static readonly Skill[] AllSkills =
        { Skill.Tread, Skill.Shadow, Skill.Fingers, Skill.Blade, Skill.Venom, Skill.Sinew };

    //                                              Tread Shadow Fingers Blade Venom Sinew
    public static readonly TradeDef[] Trades =
    {
        new("cutpurse",  new[] { 4, 3, 5, 1, 1, 2 }),
        new("garrotter", new[] { 3, 3, 1, 5, 1, 3 }),
        new("poisoner",  new[] { 2, 3, 3, 1, 5, 2 }),
        new("roofrat",   new[] { 3, 4, 2, 2, 1, 4 }),
    };

    public static readonly CostumeDef[] Costumes =
    {
        new("hangman", Visibility: 0.80f, Noise: 1.10f, BeltSlots: 3, new[]
        {
            new DyeDef("hangman_pitch", new Color("15151a"), new Color("3a3a40")),
            new DyeDef("hangman_blood", new Color("2a1012"), new Color("5c1a1a")),
            new DyeDef("hangman_moss",  new Color("1a2119"), new Color("3d4a32")),
        }),
        new("tanner", Visibility: 1.00f, Noise: 1.25f, BeltSlots: 6, new[]
        {
            new DyeDef("tanner_tar",  new Color("1e1712"), new Color("4a3a2a")),
            new DyeDef("tanner_gall", new Color("3a3320"), new Color("6b5a2e")),
            new DyeDef("tanner_raw",  new Color("4a3628"), new Color("7a5a44")),
        }),
        new("bearer", Visibility: 1.15f, Noise: 0.80f, BeltSlots: 4, new[]
        {
            new DyeDef("bearer_lime", new Color("8a8676"), new Color("b9b39c")),
            new DyeDef("bearer_ash",  new Color("4b4a48"), new Color("77746e")),
            new DyeDef("bearer_clay", new Color("5a4a3c"), new Color("86705a")),
        }),
        new("friar", Visibility: 0.95f, Noise: 1.00f, BeltSlots: 5, new[]
        {
            new DyeDef("friar_soot",   new Color("1c1a19"), new Color("54483a")),
            new DyeDef("friar_mildew", new Color("2b3028"), new Color("5a6350")),
            new DyeDef("friar_wine",   new Color("2e151c"), new Color("5e2a38")),
        }),
    };

    public static readonly VisageDef[] Visages =
    {
        new("pox"),
        new("noose"),
        new("milkeye"),
        new("branded"),
        new("splitlip"),
        new("hollow"),
    };

    /// <summary>Street names. Translation key: ALIAS_&lt;ID&gt;.</summary>
    public static readonly string[] Aliases =
        { "needle", "crow", "tallow", "ratcatcher", "wick", "gallowsbird" };

    public static string SkillNameKey(Skill skill) => $"SKILL_{skill.ToString().ToUpperInvariant()}";
    public static string SkillHintKey(Skill skill) => $"SKILL_{skill.ToString().ToUpperInvariant()}_HINT";
    public static string AliasKey(string alias) => $"ALIAS_{alias.ToUpperInvariant()}";

    public static TradeDef Trade(string id) => Trades.FirstOrDefault(t => t.Id == id) ?? Trades[0];
    public static CostumeDef Costume(string id) => Costumes.FirstOrDefault(c => c.Id == id) ?? Costumes[0];
    public static VisageDef Visage(string id) => Visages.FirstOrDefault(v => v.Id == id) ?? Visages[0];

    public static DyeDef Dye(CostumeDef costume, string id) =>
        costume.Dyes.FirstOrDefault(d => d.Id == id) ?? costume.Dyes[0];
}
