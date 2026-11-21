using System;

namespace ThePaleStitch.Rules;

/// <summary>
/// Pure character balance: how trade skills and garb turn into the numbers the
/// stealth systems use. No Godot types; unit tested.
/// </summary>
public static class CharacterRules
{
    public const int MinSkill = 1;
    public const int MaxSkill = 5;

    /// <summary>Footstep radius multiplier. Tread 1 gives 1.17, Tread 5 gives 0.85, times the garb.</summary>
    public static float NoiseMultiplier(float garbNoise, int tread) =>
        garbNoise * (1.25f - 0.08f * Clamp(tread));

    /// <summary>Multiplier on how lit the character counts as. Shadow 1 gives 1.13, Shadow 5 gives 0.85, times the garb.</summary>
    public static float VisibilityMultiplier(float garbVisibility, int shadow) =>
        garbVisibility * (1.20f - 0.07f * Clamp(shadow));

    /// <summary>Seconds of work on an ordinary lock. Fingers 1 gives 3.9, Fingers 5 gives 1.5.</summary>
    public static float LockpickSeconds(int fingers) =>
        MathF.Max(1.0f, 4.5f - 0.6f * Clamp(fingers));

    private static int Clamp(int skill) => Math.Clamp(skill, MinSkill, MaxSkill);
}
