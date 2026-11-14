using Godot;

namespace ThePaleStitch.World;

/// <summary>
/// Something the player can act on when close: a door, a body, a purse.
/// Sits on physics layer 4 so the player's probe finds it.
/// </summary>
public partial class Interactable : Area3D
{
    [Signal] public delegate void InteractedEventHandler(Node3D who);

    /// <summary>Translation key of the verb shown in the prompt ("Pick the lock").</summary>
    [Export] public string PromptKey = "PROMPT_INTERACT";

    /// <summary>Where the first-person camera should look while this is being used.</summary>
    [Export] public NodePath LookTargetPath;

    [Export] public bool Enabled = true;

    public Vector3 LookTarget =>
        LookTargetPath != null && !LookTargetPath.IsEmpty && GetNodeOrNull<Node3D>(LookTargetPath) is { } target
            ? target.GlobalPosition
            : GlobalPosition;

    public void Interact(Node3D who)
    {
        if (Enabled) EmitSignal(SignalName.Interacted, who);
    }
}
