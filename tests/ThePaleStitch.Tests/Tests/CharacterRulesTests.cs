using ThePaleStitch.Rules;
using Xunit;

namespace ThePaleStitch.Tests;

/// <summary>The balance table of Technical Guide §17.2.</summary>
public class CharacterRulesTests
{
    [Theory]
    [InlineData(1, 1.17f)]
    [InlineData(2, 1.09f)]
    [InlineData(3, 1.01f)]
    [InlineData(4, 0.93f)]
    [InlineData(5, 0.85f)]
    public void Tread_scales_footsteps(int tread, float expected) =>
        Assert.Equal(expected, CharacterRules.NoiseMultiplier(1f, tread), 2);

    [Theory]
    [InlineData(1, 1.13f)]
    [InlineData(3, 0.99f)]
    [InlineData(4, 0.92f)]
    [InlineData(5, 0.85f)]
    public void Shadow_scales_visibility(int shadow, float expected) =>
        Assert.Equal(expected, CharacterRules.VisibilityMultiplier(1f, shadow), 2);

    [Theory]
    [InlineData(1, 3.9f)]
    [InlineData(2, 3.3f)]
    [InlineData(3, 2.7f)]
    [InlineData(5, 1.5f)]
    public void Fingers_shorten_the_lock(int fingers, float expected) =>
        Assert.Equal(expected, CharacterRules.LockpickSeconds(fingers), 2);

    [Fact]
    public void Garb_multiplies_the_trade()
    {
        // Cutpurse in corpse-bearer's rags, and roof rat in the hangman's cloak (guide §17.2).
        Assert.Equal(0.74, CharacterRules.NoiseMultiplier(0.80f, 4), 2);
        Assert.Equal(1.14, CharacterRules.VisibilityMultiplier(1.15f, 3), 2);
        Assert.Equal(1.11, CharacterRules.NoiseMultiplier(1.10f, 3), 2);
        Assert.Equal(0.74, CharacterRules.VisibilityMultiplier(0.80f, 4), 2);
    }

    [Fact]
    public void Skills_out_of_range_are_clamped()
    {
        Assert.Equal(CharacterRules.NoiseMultiplier(1f, 5), CharacterRules.NoiseMultiplier(1f, 9));
        Assert.Equal(CharacterRules.LockpickSeconds(1), CharacterRules.LockpickSeconds(0));
    }
}
