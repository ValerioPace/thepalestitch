using System;

namespace ThePaleStitch.Rules;

/// <summary>
/// Pure stealth arithmetic: no Godot types, no state. Everything here is
/// covered by the unit tests in tests/ThePaleStitch.Tests, so the numbers the
/// game uses and the numbers the documentation quotes cannot drift apart.
/// </summary>
public static class StealthRules
{
    /// <summary>At full ambient cover a noise keeps only this share of its radius (1 − 0.6).</summary>
    public const float MaxMaskCut = 0.6f;

    /// <summary>Share of a noise's radius that survives each wall between source and listener.</summary>
    public const float WallDamping = 0.45f;

    /// <summary>Hearing alone never raises suspicion above this: an alarm needs eyes.</summary>
    public const float HearingCap = 0.9f;

    /// <summary>How much a very close noise adds on top of the investigation threshold.</summary>
    public const float HearingCloseBonus = 0.3f;

    /// <summary>Radius of a noise after ambient cover (a tolling bell, a crowd).</summary>
    /// <param name="radius">Radius in open, silent air, metres.</param>
    /// <param name="ambientMask">0 = silence, 1 = loudest cover.</param>
    public static float MaskedRadius(float radius, float ambientMask) =>
        radius * (1f - MaxMaskCut * Math.Clamp(ambientMask, 0f, 1f));

    /// <summary>Radius left after the noise crosses <paramref name="walls"/> walls.</summary>
    public static float AfterWalls(float radius, int walls) =>
        radius * MathF.Pow(WallDamping, Math.Max(0, walls));

    /// <summary>
    /// Suspicion of a guard who has just heard a noise. Never lowers the
    /// current value (FIX-003) and never reaches the alarm on its own.
    /// </summary>
    /// <param name="current">Suspicion before the noise, 0..1.</param>
    /// <param name="distance">Distance from the noise to the guard's ears, metres.</param>
    /// <param name="radius">Radius the noise was emitted with, metres.</param>
    /// <param name="investigateAt">Suspicion at which the guard goes to look.</param>
    public static float SuspicionAfterHearing(float current, float distance, float radius, float investigateAt)
    {
        float closeness = Math.Clamp(1f - distance / MathF.Max(radius, 0.01f), 0f, 1f);
        float fromNoise = MathF.Min(HearingCap, investigateAt + HearingCloseBonus * closeness);
        return MathF.Max(current, fromNoise);
    }
}
