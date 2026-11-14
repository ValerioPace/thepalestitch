using Godot;

namespace ThePaleStitch.UI;

/// <summary>A skill value drawn as stitches: sewn ones count, bare holes do not.</summary>
public partial class PipBar : Control
{
    [Export] public int Max = 5;
    [Export] public Color Thread = new("d9d2c0");
    [Export] public Color Hole = new("4d4a42");

    private int _value;

    [Export]
    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            QueueRedraw();
        }
    }

    public override Vector2 _GetMinimumSize() => new(Max * 30f, 28f);

    public override void _Draw()
    {
        float cell = Size.X / Max;
        float midY = Size.Y / 2f;
        for (int i = 0; i < Max; i++)
        {
            var centre = new Vector2(cell * (i + 0.5f), midY);
            if (i < _value)
            {
                var a = new Vector2(7f, 9f);
                var b = new Vector2(-7f, 9f);
                DrawLine(centre - a, centre + a, Thread, 3f, true);
                DrawLine(centre - b, centre + b, Thread, 3f, true);
            }
            else
            {
                DrawCircle(centre, 2.5f, Hole);
            }
        }
    }
}
