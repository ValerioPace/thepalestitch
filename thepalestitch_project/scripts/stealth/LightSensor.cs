using Godot;

namespace ThePaleStitch.Stealth;

/// <summary>
/// Measures how lit its parent is, from 0 (pitch dark) to 1 (standing in a torch).
///
/// Every Light3D in the group "stealth_light" counts. A light contributes only
/// if nothing on the world layer stands between it and the sample points, so
/// the shadow of a wall is a real place to hide. This is game logic, not the
/// renderer: tune it with the exported values until it agrees with what the
/// player sees on screen.
/// </summary>
public partial class LightSensor : Node3D
{
    public const string LightGroup = "stealth_light";

    /// <summary>Light that reaches everywhere, even the deepest alley.</summary>
    [Export] public float AmbientFloor = 0.04f;
    /// <summary>Weight of omni and spot lights (torches, lanterns, candles).</summary>
    [Export] public float LocalLightWeight = 0.5f;
    /// <summary>Weight of directional lights (the moon).</summary>
    [Export] public float SkyLightWeight = 0.5f;
    /// <summary>Heights above the parent's origin that are tested: shins, chest, head.</summary>
    [Export] public float[] SampleHeights = { 0.3f, 1.0f, 1.6f };
    /// <summary>Physics frames between two measurements.</summary>
    [Export] public int UpdateEvery = 3;
    [Export] public float Smoothing = 6f;

    /// <summary>Smoothed exposure, 0..1.</summary>
    public float Exposure { get; private set; }

    private float _target;
    private int _frame;
    private readonly Godot.Collections.Array<Rid> _exclude = new();

    public override void _Ready()
    {
        if (GetParent() is CollisionObject3D body)
            _exclude.Add(body.GetRid());
    }

    public override void _PhysicsProcess(double delta)
    {
        if (++_frame % Mathf.Max(1, UpdateEvery) == 0)
            _target = Measure();
        Exposure = Mathf.Lerp(Exposure, _target, 1f - Mathf.Exp(-Smoothing * (float)delta));
    }

    private float Measure()
    {
        var space = GetWorld3D().DirectSpaceState;
        Vector3 origin = GetParent<Node3D>().GlobalPosition;
        float total = 0f;

        foreach (float height in SampleHeights)
        {
            Vector3 point = origin + Vector3.Up * height;
            float sum = AmbientFloor;

            foreach (Node node in GetTree().GetNodesInGroup(LightGroup))
            {
                if (node is not Light3D light || !light.IsVisibleInTree()) continue;
                sum += Contribution(space, light, point);
            }

            total += Mathf.Clamp(sum, 0f, 1f);
        }

        return total / SampleHeights.Length;
    }

    private float Contribution(PhysicsDirectSpaceState3D space, Light3D light, Vector3 point)
    {
        switch (light)
        {
            case DirectionalLight3D sun:
            {
                // A directional light shines along its -Z, so the sky is towards +Z.
                Vector3 towardsLight = sun.GlobalBasis.Z.Normalized();
                return Blocked(space, point, point + towardsLight * 80f) ? 0f : sun.LightEnergy * SkyLightWeight;
            }
            case OmniLight3D omni:
            {
                float falloff = Falloff(point.DistanceTo(omni.GlobalPosition), omni.OmniRange);
                if (falloff <= 0f || Blocked(space, point, omni.GlobalPosition)) return 0f;
                return omni.LightEnergy * falloff * LocalLightWeight;
            }
            case SpotLight3D spot:
            {
                Vector3 toPoint = point - spot.GlobalPosition;
                float falloff = Falloff(toPoint.Length(), spot.SpotRange);
                if (falloff <= 0f) return 0f;
                float angle = Mathf.RadToDeg((-spot.GlobalBasis.Z).AngleTo(toPoint));
                if (angle > spot.SpotAngle || Blocked(space, point, spot.GlobalPosition)) return 0f;
                return spot.LightEnergy * falloff * LocalLightWeight;
            }
            default:
                return 0f;
        }
    }

    private static float Falloff(float distance, float range)
    {
        if (distance >= range) return 0f;
        float t = 1f - distance / range;
        return t * t;
    }

    private bool Blocked(PhysicsDirectSpaceState3D space, Vector3 from, Vector3 to)
    {
        var query = PhysicsRayQueryParameters3D.Create(from, to, NoiseBus.WorldMask, _exclude);
        return space.IntersectRay(query).Count > 0;
    }
}
