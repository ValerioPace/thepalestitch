using Godot;
using ThePaleStitch.Core;

namespace ThePaleStitch.Player;

/// <summary>
/// The two eyes of the game. Most of the time an orthographic camera looks
/// down at the player from one of four corners. For the actions that matter
/// (a lock, a keyhole, a throat) the view cuts into the character's head.
/// </summary>
public partial class CameraRig : Node3D
{
    [Signal] public delegate void ViewChangedEventHandler(bool firstPerson);

    public static CameraRig Current { get; private set; }

    [Export] public NodePath TargetPath;
    [Export] public float PitchDegrees = -35f;
    [Export] public float Distance = 40f;
    [Export] public float OrthoSize = 17f;
    [Export] public float FollowSpeed = 6f;
    [Export] public float TurnSpeed = 8f;
    [Export] public float StartYawDegrees = 45f;
    /// <summary>Lets the player enter first person at will to look around (legs locked).</summary>
    [Export] public bool AllowFreeLook = true;
    [Export] public float MouseSensitivity = 0.0025f;

    /// <summary>Current yaw of the isometric view, radians. Movement input is rotated by this.</summary>
    public float Yaw => Rotation.Y;

    public bool FirstPerson { get; private set; }

    private Camera3D _iso;
    private PlayerController _player;
    private float _targetYaw;
    private bool _scripted;
    private float _headYaw;
    private float _headPitch;

    public override void _EnterTree() => Current = this;

    public override void _ExitTree()
    {
        if (Current == this) Current = null;
    }

    public override void _Ready()
    {
        // The rig moves every rendered frame, following the player's interpolated
        // position: it must not be interpolated again (FIX-002).
        PhysicsInterpolationMode = PhysicsInterpolationModeEnum.Off;
        _iso = GetNode<Camera3D>("IsoCamera");
        _player = GetNodeOrNull<PlayerController>(TargetPath);

        float pitch = Mathf.DegToRad(-PitchDegrees);
        _iso.Projection = Camera3D.ProjectionType.Orthogonal;
        _iso.Size = OrthoSize;
        _iso.Position = new Vector3(0f, Mathf.Sin(pitch) * Distance, Mathf.Cos(pitch) * Distance);
        _iso.RotationDegrees = new Vector3(PitchDegrees, 0f, 0f);
        _iso.MakeCurrent();

        _targetYaw = Mathf.DegToRad(StartYawDegrees);
        Rotation = new Vector3(0f, _targetYaw, 0f);
        if (_player != null) GlobalPosition = _player.GlobalPosition;
    }

    public override void _Process(double delta)
    {
        if (_player == null) return;
        float dt = (float)delta;
        // Follow what is drawn (the interpolated body), not the last physics step,
        // or the player would stutter against the camera on fast monitors.
        Vector3 target = _player.GetGlobalTransformInterpolated().Origin;
        GlobalPosition = GlobalPosition.Lerp(target, 1f - Mathf.Exp(-FollowSpeed * dt));
        Rotation = new Vector3(0f, Mathf.LerpAngle(Rotation.Y, _targetYaw, 1f - Mathf.Exp(-TurnSpeed * dt)), 0f);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (_player == null) return;

        if (FirstPerson)
        {
            if (@event is InputEventMouseMotion motion && Input.MouseMode == Input.MouseModeEnum.Captured)
            {
                _headYaw = Mathf.Clamp(_headYaw - motion.Relative.X * MouseSensitivity, -1.2f, 1.2f);
                _headPitch = Mathf.Clamp(_headPitch - motion.Relative.Y * MouseSensitivity, -0.7f, 0.7f);
                ApplyHead();
            }
            if (!_scripted && @event.IsActionPressed(InputActions.ToggleView))
                ExitFirstPerson();
            return;
        }

        if (@event.IsActionPressed(InputActions.CameraLeft)) _targetYaw -= Mathf.Pi / 2f;
        else if (@event.IsActionPressed(InputActions.CameraRight)) _targetYaw += Mathf.Pi / 2f;
        else if (AllowFreeLook && !_player.InputLocked && @event.IsActionPressed(InputActions.ToggleView))
            EnterFirstPerson(null, scripted: false);
    }

    /// <summary>
    /// Cut to the character's eyes. With <paramref name="lookAt"/> the head starts
    /// turned to that point. A scripted view can only be closed by code.
    /// </summary>
    public void EnterFirstPerson(Vector3? lookAt, bool scripted)
    {
        if (_player == null || FirstPerson) return;
        FirstPerson = true;
        _scripted = scripted;
        _player.InputLocked = true;
        _player.Visual.Visible = false;

        // The head's rest pose looks where the body last faced.
        float bodyYaw = _player.Visual.Rotation.Y;
        _headYaw = 0f;
        _headPitch = 0f;
        if (lookAt is { } point)
        {
            Vector3 to = point - _player.Head.GlobalPosition;
            bodyYaw = Mathf.Atan2(-to.X, -to.Z);
            _headPitch = Mathf.Clamp(Mathf.Atan2(to.Y, new Vector2(to.X, to.Z).Length()), -0.7f, 0.7f);
            _player.Visual.Rotation = new Vector3(0f, bodyYaw, 0f);
        }
        _restYaw = bodyYaw;
        ApplyHead();

        _player.FirstPersonCamera.MakeCurrent();
        Mouse.SetMode(Input.MouseModeEnum.Captured);
        EmitSignal(SignalName.ViewChanged, true);
    }

    public void ExitFirstPerson()
    {
        if (_player == null || !FirstPerson) return;
        FirstPerson = false;
        _scripted = false;
        _player.InputLocked = false;
        _player.Visual.Visible = true;
        _iso.MakeCurrent();
        Mouse.SetMode(Input.MouseModeEnum.Visible);
        EmitSignal(SignalName.ViewChanged, false);
    }

    private float _restYaw;

    private void ApplyHead() =>
        _player.Head.Rotation = new Vector3(_headPitch, _restYaw + _headYaw, 0f);
}
