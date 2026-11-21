using Godot;

namespace ThePaleStitch.Stealth;

/// <summary>
/// Draws every noise as a pale ring spreading on the ground, as wide as the
/// noise carries. In the isometric view this is how the player reads sound.
/// </summary>
public partial class NoiseRingSpawner : Node3D
{
    [Export] public Color RingColor = new(0.85f, 0.82f, 0.72f, 0.55f);
    [Export] public float Seconds = 0.7f;
    /// <summary>Noises smaller than this are not worth a ring.</summary>
    [Export] public float MinRadius = 0.8f;

    private TorusMesh _mesh;

    public override void _EnterTree() => NoiseBus.Emitted += OnNoise;
    public override void _ExitTree() => NoiseBus.Emitted -= OnNoise;

    public override void _Ready()
    {
        // Rings are animated by tweens every frame, not by physics.
        PhysicsInterpolationMode = PhysicsInterpolationModeEnum.Off;
        _mesh = new TorusMesh { InnerRadius = 0.965f, OuterRadius = 1.0f, Rings = 64, RingSegments = 4 };
    }

    private void OnNoise(NoiseEvent noise)
    {
        if (noise.Radius < MinRadius) return;

        var material = new StandardMaterial3D
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            AlbedoColor = RingColor,
            DisableReceiveShadows = true,
        };
        var ring = new MeshInstance3D
        {
            Mesh = _mesh,
            MaterialOverride = material,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
        };
        AddChild(ring);
        ring.GlobalPosition = new Vector3(noise.Origin.X, noise.Origin.Y + 0.06f, noise.Origin.Z);
        ring.Scale = new Vector3(0.2f, 0.05f, 0.2f);

        var faded = RingColor with { A = 0f };
        var tween = ring.CreateTween().SetParallel();
        tween.TweenProperty(ring, "scale", new Vector3(noise.Radius, 0.05f, noise.Radius), Seconds)
            .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
        tween.TweenProperty(material, "albedo_color", faded, Seconds)
            .SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.In);
        tween.Chain().TweenCallback(Callable.From(ring.QueueFree));
    }
}
