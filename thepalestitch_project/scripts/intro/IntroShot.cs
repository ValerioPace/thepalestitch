using Godot;

namespace ThePaleStitch.Intro;

/// <summary>
/// One shot of the opening film. Four Marker3D children define it and can be
/// dragged in the editor: the camera travels from CamFrom to CamTo while it
/// looks from LookFrom to LookTo.
/// </summary>
public partial class IntroShot : Node3D
{
    [Export] public float Duration = 12f;
    [Export] public float Fov = 50f;
    /// <summary>Narration line (translation key). Empty for a silent shot.</summary>
    [Export] public string SubtitleKey = "";
    /// <summary>Place-and-date card shown at the start of the shot (translation key).</summary>
    [Export] public string CardKey = "";

    public Vector3 CameraAt(float t) =>
        GetNode<Node3D>("CamFrom").GlobalPosition.Lerp(GetNode<Node3D>("CamTo").GlobalPosition, t);

    public Vector3 TargetAt(float t) =>
        GetNode<Node3D>("LookFrom").GlobalPosition.Lerp(GetNode<Node3D>("LookTo").GlobalPosition, t);
}
