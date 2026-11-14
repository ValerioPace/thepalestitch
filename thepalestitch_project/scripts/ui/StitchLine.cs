using Godot;

namespace ThePaleStitch.UI;

/// <summary>
/// The game's mark: a wound drawn across the control and sewn shut, stitch by
/// stitch, in pale thread. Used under the title and in the opening film.
/// </summary>
public partial class StitchLine : Control
{
    [Export] public Color Thread = new("d9d2c0");
    [Export] public Color Wound = new("4a1512");
    [Export] public int Stitches = 23;
    [Export] public float StitchHeight = 26f;
    [Export] public float ThreadWidth = 3f;
    /// <summary>Seconds to sew the whole line when <see cref="AutoPlay"/> is on.</summary>
    [Export] public float SewSeconds = 3.0f;
    [Export] public bool AutoPlay = true;
    [Export] public int Seed = 1352;

    private float _progress;

    /// <summary>0 = open wound, 1 = fully sewn.</summary>
    [Export(PropertyHint.Range, "0,1")]
    public float Progress
    {
        get => _progress;
        set
        {
            _progress = Mathf.Clamp(value, 0f, 1f);
            QueueRedraw();
        }
    }

    public override void _Ready()
    {
        if (!AutoPlay) return;
        Progress = 0f;
        CreateTween().TweenProperty(this, "Progress", 1f, SewSeconds).SetDelay(0.6f);
    }

    public override void _Draw()
    {
        if (Stitches < 1) return;
        var random = new RandomNumberGenerator { Seed = (ulong)Seed };
        float midY = Size.Y / 2f;
        float step = Size.X / (Stitches + 1);

        // The wound: a thin, uneven dark line.
        var wound = new Vector2[Stitches + 2];
        for (int i = 0; i < wound.Length; i++)
            wound[i] = new Vector2(step * i, midY + random.RandfRange(-3f, 3f));
        DrawPolyline(wound, Wound, 2.5f, true);

        // The thread: slanted stitches crossing it, with a run of thread between them.
        float sewn = _progress * Stitches;
        Vector2? previousBottom = null;
        for (int i = 0; i < Stitches; i++)
        {
            float lean = random.RandfRange(-5f, 5f);
            float height = StitchHeight * random.RandfRange(0.8f, 1.15f);
            float amount = Mathf.Clamp(sewn - i, 0f, 1f);
            if (amount <= 0f) continue;

            float x = step * (i + 1);
            var top = new Vector2(x - 6f + lean, midY - height / 2f);
            var bottom = new Vector2(x + 6f + lean, midY + height / 2f);

            if (previousBottom is { } from)
                DrawLine(from, top, Thread with { A = 0.35f }, 1.5f, true);
            DrawLine(top, top.Lerp(bottom, amount), Thread, ThreadWidth, true);
            previousBottom = bottom;
        }
    }
}
