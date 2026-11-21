namespace ThePaleStitch.Core;

/// <summary>
/// The physics layers of the game as named constants: the single source of
/// truth for code (CR-008). Their names are declared in project.godot
/// ([layer_names]) and the unit tests check that both, and the layers and masks
/// written in the scenes, agree with these values.
///
/// Pure C#: no Godot types, so the tests can compile it in.
/// </summary>
public static class PhysicsLayers
{
    /// <summary>Layer 1. Every static body of a level: walls, floors, cover.</summary>
    public const uint World = 1u << 0;
    /// <summary>Layer 2. The player character.</summary>
    public const uint Player = 1u << 1;
    /// <summary>Layer 3. Guards and other hostile characters.</summary>
    public const uint Guards = 1u << 2;
    /// <summary>Layer 4. Things the player can use (doors, bodies, purses).</summary>
    public const uint Interactables = 1u << 3;
    /// <summary>Layer 5. Floor zones with their own footstep loudness.</summary>
    public const uint Surfaces = 1u << 4;

    /// <summary>What the player bumps into: the world and the guards (FIX-001).</summary>
    public const uint PlayerMask = World | Guards;
    /// <summary>What a guard bumps into: the world and the player (FIX-001).</summary>
    public const uint GuardMask = World | Player;
    /// <summary>What stops light, sight and sound: only the world.</summary>
    public const uint OccluderMask = World;

    /// <summary>Names as declared in project.godot, index 0 = layer 1.</summary>
    public static readonly string[] Names = { "world", "player", "guards", "interactables", "surfaces" };
}
