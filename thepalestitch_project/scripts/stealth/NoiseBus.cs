using System;
using Godot;
using ThePaleStitch.Core;
using ThePaleStitch.Rules;

namespace ThePaleStitch.Stealth;

public enum NoiseKind
{
    Footstep,
    Tool,
    Impact,
    Voice,
}

/// <summary>One sound made in the world: where, how far it carries in open air, who made it.</summary>
public readonly record struct NoiseEvent(Vector3 Origin, float Radius, NoiseKind Kind, Node3D Source);

/// <summary>
/// The sound propagation model, first pass.
///
/// A noise is a sphere of a given radius. Before it is emitted it is shrunk by
/// the ambient mask (rain, bells, a crowd). Each listener then asks
/// <see cref="AudibleRadius"/> how far that noise reaches *towards them*: every
/// wall on the straight line between source and listener damps it.
///
/// Subscribers must unsubscribe in _ExitTree: the event is static and outlives scenes.
/// </summary>
public static class NoiseBus
{
    private const int MaxWalls = 3;

    public static event Action<NoiseEvent> Emitted;

    /// <summary>0 = dead silence, 1 = the loudest ambient cover (a tolling bell overhead).</summary>
    public static float AmbientMask { get; set; }

    /// <summary>Call when a level starts so nothing leaks from the previous one.</summary>
    public static void Reset() => AmbientMask = 0f;

    /// <summary>Radius a noise of <paramref name="radius"/> has right now, after the ambient mask.</summary>
    public static float Masked(float radius) => StealthRules.MaskedRadius(radius, AmbientMask);

    /// <summary>
    /// Publish a noise. Returns the radius it was emitted with (after the
    /// ambient mask), so the emitter can show the same number the guards get.
    /// </summary>
    public static float Emit(Vector3 origin, float radius, NoiseKind kind, Node3D source)
    {
        float masked = Masked(radius);
        if (masked > 0.05f)
            Emitted?.Invoke(new NoiseEvent(origin, masked, kind, source));
        return masked;
    }

    /// <summary>
    /// Radius of <paramref name="noise"/> in the direction of <paramref name="listener"/>,
    /// after the walls in between. The listener hears it when this is at least the distance.
    /// </summary>
    public static float AudibleRadius(PhysicsDirectSpaceState3D space, NoiseEvent noise, Vector3 listener)
    {
        float radius = noise.Radius;
        Vector3 from = noise.Origin + Vector3.Up * 1.0f;
        Vector3 to = listener;
        var exclude = new Godot.Collections.Array<Rid>();

        for (int wall = 0; wall < MaxWalls; wall++)
        {
            var query = PhysicsRayQueryParameters3D.Create(from, to, PhysicsLayers.OccluderMask, exclude);
            var hit = space.IntersectRay(query);
            if (hit.Count == 0) break;

            radius *= StealthRules.WallDamping;
            exclude.Add(hit["rid"].AsRid());
        }

        return radius;
    }

    public static bool CanHear(PhysicsDirectSpaceState3D space, NoiseEvent noise, Vector3 listener) =>
        AudibleRadius(space, noise, listener) >= noise.Origin.DistanceTo(listener);
}
