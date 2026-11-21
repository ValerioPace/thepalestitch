using ThePaleStitch.Rules;
using Xunit;

namespace ThePaleStitch.Tests;

public class StealthRulesTests
{
    [Theory]
    [InlineData(10f, 0f, 10f)]
    [InlineData(10f, 1f, 4f)]
    [InlineData(10f, 0.5f, 7f)]
    [InlineData(10f, 2f, 4f)]      // mask is clamped to 1
    [InlineData(10f, -1f, 10f)]    // and to 0
    public void MaskedRadius_scales_with_ambient_cover(float radius, float mask, float expected) =>
        Assert.Equal(expected, StealthRules.MaskedRadius(radius, mask), 4);

    [Theory]
    [InlineData(0, 10f)]
    [InlineData(1, 4.5f)]
    [InlineData(2, 2.025f)]
    [InlineData(-3, 10f)]
    public void AfterWalls_keeps_45_percent_per_wall(int walls, float expected) =>
        Assert.Equal(expected, StealthRules.AfterWalls(10f, walls), 4);

    [Fact]
    public void Hearing_never_lowers_suspicion()   // FIX-003
    {
        float after = StealthRules.SuspicionAfterHearing(0.95f, distance: 0.5f, radius: 8f, investigateAt: 0.4f);
        Assert.Equal(0.95, after, 4);
    }

    [Fact]
    public void Hearing_alone_never_reaches_the_alarm()
    {
        float after = StealthRules.SuspicionAfterHearing(0f, distance: 0f, radius: 8f, investigateAt: 0.8f);
        Assert.Equal(StealthRules.HearingCap, after, 4);
    }

    [Theory]
    [InlineData(0f, 0.7f)]     // right on top of the guard: 0.4 + 0.3
    [InlineData(4f, 0.55f)]    // half way to the edge of hearing
    [InlineData(8f, 0.4f)]     // at the edge: just enough to go and look
    [InlineData(12f, 0.4f)]    // beyond (walls already decided it was heard)
    public void Hearing_raises_suspicion_with_closeness(float distance, float expected) =>
        Assert.Equal(expected, StealthRules.SuspicionAfterHearing(0f, distance, 8f, 0.4f), 4);

    [Fact]
    public void Hearing_survives_a_zero_radius()
    {
        float after = StealthRules.SuspicionAfterHearing(0f, 1f, 0f, 0.4f);
        Assert.InRange(after, 0.4f, 0.9f);
    }

    /// <summary>The worked example of Technical Guide §21.3, so the guide cannot go stale.</summary>
    [Fact]
    public void Guide_example_cutpurse_on_gravel()
    {
        float multiplier = CharacterRules.NoiseMultiplier(garbNoise: 1.10f, tread: 4); // hangman's cloak
        float radius = 4.5f * 1.60f * multiplier;                                       // walking on gravel
        Assert.Equal(7.37, radius, 2);
        Assert.Equal(3.31, StealthRules.AfterWalls(radius, 1), 2);
        Assert.Equal(2.95, StealthRules.MaskedRadius(radius, 1f), 2);
    }
}
