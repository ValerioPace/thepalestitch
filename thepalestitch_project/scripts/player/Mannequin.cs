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

    public override void _Ready()
    {
        // Turned every rendered frame on the creation screen: no physics interpolation.
        if (TurnSpeed != 0f) PhysicsInterpolationMode = PhysicsInterpolationModeEnum.Off;
    }

    public override void _Process(double delta)
    {
        if (TurnSpeed != 0f)
            RotateY(Mathf.DegToRad(TurnSpeed) * (float)delta);
    }

    public void Apply(DyeDef dye)
    {
        Paint("Body", dye.Cloth);
        Paint("Hood", dye.Cloth.Darkened(0.15f));
        Paint("Belt", dye.Trim);
        Paint("Cowl", dye.Trim.Darkened(0.2f));
    }

    private void Paint(string nodeName, Color color)
    {
        if (GetNodeOrNull<MeshInstance3D>(nodeName) is not { } mesh) return;
        mesh.MaterialOverride = new StandardMaterial3D { AlbedoColor = color, Roughness = 0.95f };
    }
}
