using Godot;

namespace ThePaleStitch.Character;

/// <summary>
/// The choices made in the creation screen, plus the numbers the stealth
/// systems derive from them. Balance lives here, in one place.
/// </summary>
public sealed class CharacterProfile
{
    public string TradeId { get; set; }
    public string CostumeId { get; set; }
    public string VariantId { get; set; }
    public string VisageId { get; set; }
    public string Alias { get; set; }

    public TradeDef Trade => CharacterCatalog.Trade(TradeId);
    public CostumeDef Costume => CharacterCatalog.Costume(CostumeId);
    public VariantDef Variant => CharacterCatalog.Variant(Costume, VariantId);
    public VisageDef Visage => CharacterCatalog.Visage(VisageId);

    public static CharacterProfile Default() => new()
    {
        TradeId = CharacterCatalog.Trades[0].Id,
        CostumeId = CharacterCatalog.Costumes[0].Id,
        VariantId = CharacterCatalog.Costumes[0].Variants[0].Id,
        VisageId = CharacterCatalog.Visages[0].Id,
        Alias = CharacterCatalog.Aliases[0],
    };

    public CharacterProfile Clone() => (CharacterProfile)MemberwiseClone();

    /// <summary>Footstep radius multiplier. Tread 1 gives 1.17, Tread 5 gives 0.85, times the garb.</summary>
    public float NoiseMultiplier => Costume.Noise * (1.25f - 0.08f * Trade[Skill.Tread]);

    /// <summary>Multiplier on how lit the character counts as. Shadow 1 gives 1.13, Shadow 5 gives 0.85, times the garb.</summary>
    public float VisibilityMultiplier => Costume.Visibility * (1.20f - 0.07f * Trade[Skill.Shadow]);

    /// <summary>Seconds of work on an ordinary lock. Fingers 1 gives 3.9, Fingers 5 gives 1.5.</summary>
    public float LockpickSeconds => Mathf.Max(1.0f, 4.5f - 0.6f * Trade[Skill.Fingers]);
}
