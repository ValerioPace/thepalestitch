using Godot;
using ThePaleStitch.Character;

namespace ThePaleStitch.Player;

/// <summary>
/// Graybox body: a hooded, hunched shape that takes the colours of the chosen
/// garb. Stands in for the real model in the creation screen and in play.
/// Replace the meshes, keep <see cref="Apply"/>.
/// </summary>
public partial class Mannequin : Node3D
{
    /// <summary>Degrees per second. Used by the creation screen to show the figure all round.</summary>
    [Export] public float TurnSpeed;

    public override void _Process(double delta)
    {
        if (TurnSpeed != 0f)
            RotateY(Mathf.DegToRad(TurnSpeed) * (float)delta);
    }

    public void Apply(VariantDef variant)
    {
        Paint("Body", variant.Cloth);
        Paint("Hood", variant.Cloth.Darkened(0.15f));
        Paint("Belt", variant.Trim);
        Paint("Cowl", variant.Trim.Darkened(0.2f));
    }

    private void Paint(string nodeName, Color color)
    {
        if (GetNodeOrNull<MeshInstance3D>(nodeName) is not { } mesh) return;
        mesh.MaterialOverride = new StandardMaterial3D { AlbedoColor = color, Roughness = 0.95f };
    }
}
