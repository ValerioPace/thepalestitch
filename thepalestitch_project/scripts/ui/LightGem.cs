using Godot;

namespace ThePaleStitch.UI;

/// <summary>
/// The eye at the bottom of the screen. Shut and dark when the player is
/// hidden, wide and bone-pale when he stands in the light.
/// </summary>
public partial class LightGem : Control
{
    [Export] public Color Dark = new("0b0b0d");
    [Export] public Color Lit = new("e9e2cb");
    [Export] public Color Rim = new("6b6557");

    /// <summary>0..1, the player's visibility.</summary>
    public float Value { get; set; }

    private float _shown;

    public override void _Process(double delta)
    {
        float next = Mathf.Lerp(_shown, Value, 1f - Mathf.Exp(-8f * (float)delta));
        if (Mathf.IsEqualApprox(next, _shown)) return;
        _shown = next;
        QueueRedraw();
    }

    public override void _Draw()
    {
        Vector2 centre = Size / 2f;
        float halfWidth = Size.X * 0.48f;
        // The lids open with the light.
        float halfHeight = Size.Y * Mathf.Lerp(0.16f, 0.46f, _shown);

        // Upper lid left to right, lower lid back again. The two corners are shared,
        // so the lower lid skips them: duplicate points break triangulation.
        const int steps = 24;
        var outline = new Vector2[steps * 2 - 2];
        for (int i = 0; i < steps; i++)
        {
            float t = i / (float)(steps - 1);
            float x = Mathf.Lerp(-halfWidth, halfWidth, t);
            float y = Mathf.Sin(t * Mathf.Pi) * halfHeight;
            outline[i] = centre + new Vector2(x, -y);
            if (i > 0 && i < steps - 1)
                outline[steps * 2 - 2 - i] = centre + new Vector2(x, y);
        }

        DrawColoredPolygon(outline, Dark.Lerp(Lit, _shown));

        var closed = new Vector2[outline.Length + 1];
        outline.CopyTo(closed, 0);
        closed[^1] = outline[0];
        DrawPolyline(closed, Rim, 2f, true);

        // Slit pupil.
        float pupil = halfHeight * 0.85f;
        DrawLine(centre - new Vector2(0, pupil), centre + new Vector2(0, pupil), Dark, Mathf.Lerp(3f, 7f, _shown), true);
    }
}
