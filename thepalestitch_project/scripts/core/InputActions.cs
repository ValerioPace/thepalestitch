using Godot;

namespace ThePaleStitch.Core;

/// <summary>Names of the actions defined in Project Settings > Input Map.</summary>
public static class InputActions
{
    public static readonly StringName MoveLeft = "move_left";
    public static readonly StringName MoveRight = "move_right";
    public static readonly StringName MoveForward = "move_forward";
    public static readonly StringName MoveBack = "move_back";
    public static readonly StringName Sneak = "sneak";
    public static readonly StringName Run = "run";
    public static readonly StringName Interact = "interact";
    public static readonly StringName CameraLeft = "camera_rotate_left";
    public static readonly StringName CameraRight = "camera_rotate_right";
    public static readonly StringName ToggleView = "toggle_view";
    public static readonly StringName Pause = "pause";
    public static readonly StringName Skip = "skip";
    /// <summary>F3, debug builds only (CR-020).</summary>
    public static readonly StringName DebugOverlay = "debug_overlay";
}
