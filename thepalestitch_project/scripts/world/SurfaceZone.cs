using Godot;
using ThePaleStitch.Player;

namespace ThePaleStitch.World;

public enum SurfaceType
{
    Cobble,
    Straw,
    Gravel,
    Wood,
    Mud,
}

/// <summary>
/// An area of floor with its own footstep loudness. Where zones overlap the
/// one with the highest Area3D priority wins (planks laid over gravel).
/// Ground outside every zone counts as cobbles.
/// </summary>
public partial class SurfaceZone : Area3D
{
    [Export] public SurfaceType Surface = SurfaceType.Cobble;

    public float NoiseMultiplier => Loudness(Surface);

    public static float Loudness(SurfaceType surface) => surface switch
    {
        SurfaceType.Straw => 0.45f,
        SurfaceType.Mud => 0.70f,
        SurfaceType.Cobble => 1.00f,
        SurfaceType.Wood => 1.25f,
        SurfaceType.Gravel => 1.60f,
        _ => 1.00f,
    };

    public static string NameKey(SurfaceType surface) => $"SURFACE_{surface.ToString().ToUpperInvariant()}";

    public override void _Ready()
    {
        BodyEntered += body => { if (body is PlayerController player) player.EnterSurface(this); };
        BodyExited += body => { if (body is PlayerController player) player.ExitSurface(this); };
    }
}
