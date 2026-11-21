using Godot;
using ThePaleStitch.Core;

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
    /// <summary>One light test of the last measurement, kept for the debug overlay.</summary>
    public readonly record struct LightRay(Vector3 Point, Vector3 Light, bool Blocked, float Contribution);

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

    /// <summary>When true, the rays of each measurement are kept in <see cref="Rays"/> (debug overlay).</summary>
    public bool RecordRays { get; set; }

    public System.Collections.Generic.List<LightRay> Rays { get; } = new();

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
        if (RecordRays) Rays.Clear();

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
                Vector3 sky = point + sun.GlobalBasis.Z.Normalized() * 80f;
                bool blocked = Blocked(space, point, sky);
                return Record(point, point + sun.GlobalBasis.Z.Normalized() * 3f, blocked, blocked ? 0f : sun.LightEnergy * SkyLightWeight);
            }
            case OmniLight3D omni:
            {
                float falloff = Falloff(point.DistanceTo(omni.GlobalPosition), omni.OmniRange);
                if (falloff <= 0f) return 0f;
                bool blocked = Blocked(space, point, omni.GlobalPosition);
                return Record(point, omni.GlobalPosition, blocked, blocked ? 0f : omni.LightEnergy * falloff * LocalLightWeight);
            }
            case SpotLight3D spot:
            {
                Vector3 toPoint = point - spot.GlobalPosition;
                float falloff = Falloff(toPoint.Length(), spot.SpotRange);
                if (falloff <= 0f) return 0f;
                float angle = Mathf.RadToDeg((-spot.GlobalBasis.Z).AngleTo(toPoint));
                if (angle > spot.SpotAngle) return 0f;
                bool blocked = Blocked(space, point, spot.GlobalPosition);
                return Record(point, spot.GlobalPosition, blocked, blocked ? 0f : spot.LightEnergy * falloff * LocalLightWeight);
            }
            default:
                return 0f;
        }
    }

    private float Record(Vector3 point, Vector3 light, bool blocked, float contribution)
    {
        if (RecordRays) Rays.Add(new LightRay(point, light, blocked, contribution));
        return contribution;
    }

    private static float Falloff(float distance, float range)
    {
        if (distance >= range) return 0f;
        float t = 1f - distance / range;
        return t * t;
    }

    private bool Blocked(PhysicsDirectSpaceState3D space, Vector3 from, Vector3 to)
    {
        var query = PhysicsRayQueryParameters3D.Create(from, to, PhysicsLayers.OccluderMask, _exclude);
        return space.IntersectRay(query).Count > 0;
    }
}
