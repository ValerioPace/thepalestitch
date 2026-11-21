using ThePaleStitch.Rules;

namespace ThePaleStitch.Character;

/// <summary>
/// The choices made in the creation screen. Holds ids only, so it can be saved
/// as is; the numbers the stealth systems use come from <see cref="CharacterRules"/>.
/// </summary>
public sealed class CharacterProfile
{
    public string TradeId { get; set; }
    public string CostumeId { get; set; }
    public string DyeId { get; set; }
    public string VisageId { get; set; }
    public string Alias { get; set; }

    public TradeDef Trade => CharacterCatalog.Trade(TradeId);
    public CostumeDef Costume => CharacterCatalog.Costume(CostumeId);
    public DyeDef Dye => CharacterCatalog.Dye(Costume, DyeId);
    public VisageDef Visage => CharacterCatalog.Visage(VisageId);

    public static CharacterProfile Default() => new()
    {
        TradeId = CharacterCatalog.Trades[0].Id,
        CostumeId = CharacterCatalog.Costumes[0].Id,
        DyeId = CharacterCatalog.Costumes[0].Dyes[0].Id,
        VisageId = CharacterCatalog.Visages[0].Id,
        Alias = CharacterCatalog.Aliases[0],
    };

    public CharacterProfile Clone() => (CharacterProfile)MemberwiseClone();

    /// <summary>Footstep radius multiplier from garb and Tread.</summary>
    public float NoiseMultiplier => CharacterRules.NoiseMultiplier(Costume.Noise, Trade[Skill.Tread]);

    /// <summary>Multiplier on how lit the character counts as, from garb and Shadow.</summary>
    public float VisibilityMultiplier => CharacterRules.VisibilityMultiplier(Costume.Visibility, Trade[Skill.Shadow]);

    /// <summary>Seconds of work on an ordinary lock, from Fingers.</summary>
    public float LockpickSeconds => CharacterRules.LockpickSeconds(Trade[Skill.Fingers]);
}
