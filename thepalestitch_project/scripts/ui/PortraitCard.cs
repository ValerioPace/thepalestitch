using Godot;
using ThePaleStitch.Character;

namespace ThePaleStitch.UI;

/// <summary>
/// Placeholder portrait: a hooded bust drawn with polygons, marked by the
/// chosen visage and dyed like the chosen garb. It fixes the art direction
/// until real portraits exist: scowl, shadowed eyes, damage. No soft faces.
/// </summary>
public partial class PortraitCard : Control
{
    private static readonly Color Skin = new("8c8672");
    private static readonly Color SkinShade = new("5c5849");
    private static readonly Color Ink = new("0c0b0b");
    private static readonly Color Scar = new("6e2a22");

    private string _visage = "pox";
    private Color _cloth = new("15151a");
    private Color _trim = new("3a3a40");

    public void Portray(VisageDef visage, VariantDef variant)
    {
        _visage = visage.Id;
        // Lift the cloth a little so the hood reads against the dark card.
        _cloth = variant.Cloth.Lightened(0.08f);
        _trim = variant.Trim;
        QueueRedraw();
    }

    public override void _Draw()
    {
        DrawRect(new Rect2(Vector2.Zero, Size), new Color("0a0a0b"));

        bool hollow = _visage == "hollow";
        Color skin = hollow ? Skin.Lerp(new Color("77786f"), 0.6f) : Skin;

        // Shoulders, neck, hood.
        Poly(_cloth.Darkened(0.25f), (0.04f, 1f), (0.14f, 0.82f), (0.36f, 0.72f), (0.64f, 0.72f), (0.86f, 0.82f), (0.96f, 1f));
        Poly(SkinShade.Darkened(0.6f), (0.41f, 0.64f), (0.59f, 0.64f), (0.62f, 0.78f), (0.38f, 0.78f));
        Poly(_cloth, (0.26f, 0.74f), (0.20f, 0.46f), (0.27f, 0.22f), (0.41f, 0.09f), (0.50f, 0.06f), (0.59f, 0.09f), (0.73f, 0.22f), (0.80f, 0.46f), (0.74f, 0.74f), (0.62f, 0.66f), (0.38f, 0.66f));

        // Face: long, hard, never round.
        float jaw = hollow ? 0.035f : 0f;
        Poly(skin, (0.36f, 0.28f), (0.64f, 0.28f), (0.67f - jaw, 0.48f), (0.61f - jaw, 0.64f), (0.52f, 0.71f), (0.48f, 0.71f), (0.39f + jaw, 0.64f), (0.33f + jaw, 0.48f));
        // One side of the face always lost to shadow.
        Poly(Ink with { A = 0.38f }, (0.50f, 0.28f), (0.64f, 0.28f), (0.67f - jaw, 0.48f), (0.61f - jaw, 0.64f), (0.52f, 0.71f), (0.50f, 0.71f));
        // Hood shadow over the brow.
        Poly(Ink with { A = 0.62f }, (0.36f, 0.28f), (0.64f, 0.28f), (0.66f, 0.41f), (0.50f, 0.44f), (0.34f, 0.41f));

        if (hollow)
        {
            Poly(Ink with { A = 0.45f }, (0.37f, 0.50f), (0.44f, 0.52f), (0.43f, 0.62f), (0.39f, 0.60f));
            Poly(Ink with { A = 0.45f }, (0.63f, 0.50f), (0.56f, 0.52f), (0.57f, 0.62f), (0.61f, 0.60f));
            DrawCircle(P(0.43f, 0.455f), W(0.035f), Ink with { A = 0.5f });
            DrawCircle(P(0.57f, 0.455f), W(0.035f), Ink with { A = 0.5f });
        }

        // Brows drawn down to a scowl; eyes as slits.
        Line(Ink, 0.012f, (0.37f, 0.405f), (0.47f, 0.435f));
        Line(Ink, 0.012f, (0.63f, 0.405f), (0.53f, 0.435f));
        Line(Ink, 0.010f, (0.40f, 0.455f), (0.46f, 0.458f));
        if (_visage == "milkeye")
        {
            DrawCircle(P(0.57f, 0.457f), W(0.020f), new Color("d6d8d0"));
            Line(Scar, 0.008f, (0.585f, 0.36f), (0.56f, 0.56f));
        }
        else
        {
            Line(Ink, 0.010f, (0.54f, 0.458f), (0.60f, 0.455f));
        }
        DrawCircle(P(0.435f, 0.456f), W(0.004f), new Color("c9c2a8"));

        // Broken nose, thin mouth turned down.
        Line(Ink with { A = 0.7f }, 0.008f, (0.505f, 0.45f), (0.485f, 0.53f), (0.505f, 0.555f));
        if (_visage == "splitlip")
        {
            Line(Ink, 0.009f, (0.43f, 0.615f), (0.485f, 0.60f));
            Line(Ink, 0.009f, (0.50f, 0.612f), (0.57f, 0.62f));
            Line(Scar, 0.010f, (0.50f, 0.555f), (0.488f, 0.60f), (0.497f, 0.67f));
        }
        else
        {
            Line(Ink, 0.009f, (0.43f, 0.618f), (0.50f, 0.603f), (0.57f, 0.62f));
        }

        switch (_visage)
        {
            case "pox":
                var random = new RandomNumberGenerator { Seed = 77 };
                for (int i = 0; i < 26; i++)
                {
                    float x = random.RandfRange(0.37f, 0.63f);
                    float y = random.RandfRange(0.47f, 0.66f);
                    if (Mathf.Abs(x - 0.5f) < 0.035f && y > 0.58f) continue;
                    DrawCircle(P(x, y), W(random.RandfRange(0.004f, 0.009f)), SkinShade.Darkened(0.3f));
                }
                break;
            case "noose":
                Line(Scar, 0.022f, (0.40f, 0.70f), (0.50f, 0.735f), (0.60f, 0.70f));
                Line(Scar.Darkened(0.4f), 0.006f, (0.40f, 0.715f), (0.50f, 0.75f), (0.60f, 0.715f));
                break;
            case "branded":
                // The thief's brand, burned into the cheek.
                Line(Scar, 0.012f, (0.385f, 0.515f), (0.445f, 0.515f));
                Line(Scar, 0.012f, (0.415f, 0.515f), (0.415f, 0.595f));
                break;
        }

        // Hood edge in the trim colour, then the card frame.
        Line(_trim, 0.008f, (0.38f, 0.66f), (0.33f, 0.47f), (0.36f, 0.28f), (0.50f, 0.20f), (0.64f, 0.28f), (0.67f, 0.47f), (0.62f, 0.66f));
        DrawRect(new Rect2(Vector2.One, Size - Vector2.One * 2f), new Color("3b382f"), false, 2f);
    }

    private Vector2 P(float x, float y) => new(x * Size.X, y * Size.Y);
    private float W(float fraction) => fraction * Size.X;

    private void Poly(Color color, params (float X, float Y)[] points)
    {
        var polygon = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++) polygon[i] = P(points[i].X, points[i].Y);
        DrawColoredPolygon(polygon, color);
    }

    private void Line(Color color, float width, params (float X, float Y)[] points)
    {
        var line = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++) line[i] = P(points[i].X, points[i].Y);
        DrawPolyline(line, color, Mathf.Max(1f, W(width)), true);
    }
}
