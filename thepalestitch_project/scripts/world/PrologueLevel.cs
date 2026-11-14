using Godot;
using ThePaleStitch.AI;
using ThePaleStitch.Core;
using ThePaleStitch.Player;
using ThePaleStitch.Stealth;
using ThePaleStitch.UI;

namespace ThePaleStitch.World;

/// <summary>
/// Prologue, graybox slice: the street, the watchman, the wool merchant's door.
/// Teaches light, sound and the first-person cut, then stops at the lock.
/// What happens behind the door is described in docs/design.
/// </summary>
public partial class PrologueLevel : Node3D
{
    [Export] public NodePath PlayerPath;
    [Export] public NodePath RigPath;
    [Export] public NodePath HudPath;
    [Export] public NodePath DoorPath;
    [Export] public NodePath NavRegionPath;

    [ExportGroup("Abbey bell")]
    /// <summary>Seconds between two tollings. While it tolls, footsteps carry less.</summary>
    [Export] public float BellEvery = 32f;
    [Export] public float BellSeconds = 7f;

    [ExportGroup("Lock")]
    [Export] public float LockNoiseRadius = 2.2f;
    [Export] public float LockNoiseEvery = 0.6f;

    private PlayerController _player;
    private CameraRig _rig;
    private Hud _hud;
    private Interactable _door;

    private float _clock;
    private float _bellTimer;
    private float _bellLeft;
    private int _hintIndex;
    private bool _picking;
    private float _pickProgress;
    private float _pickNoise;
    private bool _over;

    // Tutorial lines, shown once each when the player has walked this far east.
    private static readonly (float X, string Key)[] Hints =
    {
        (-100f, "HINT_MOVE"),
        (-16f, "HINT_LIGHT"),
        (-13f, "HINT_SURFACE"),
        (-8f, "HINT_WATCH"),
        (-2f, "HINT_LOOK"),
        (15f, "HINT_DOOR"),
    };

    public override void _Ready()
    {
        NoiseBus.Reset();
        _bellTimer = BellEvery * 0.5f;

        _player = GetNode<PlayerController>(PlayerPath);
        _rig = GetNode<CameraRig>(RigPath);
        _hud = GetNode<Hud>(HudPath);
        _door = GetNode<Interactable>(DoorPath);

        // The navigation mesh is baked from the level's static bodies at load:
        // move a wall in the editor and the watch walks round it, no manual bake.
        if (GetNodeOrNull<NavigationRegion3D>(NavRegionPath) is { } region)
            region.BakeNavigationMesh(false);

        _door.Interacted += OnDoorUsed;
        _rig.ViewChanged += OnViewChanged;
        foreach (Node node in GetTree().GetNodesInGroup(Watchman.Group))
            if (node is Watchman watchman) watchman.RaisedAlarm += OnAlarm;

        _hud.SetObjective("PROLOGUE_OBJ_REACH");
    }

    public override void _ExitTree() => NoiseBus.Reset();

    public override void _Process(double delta)
    {
        if (_over) return;
        float dt = (float)delta;
        _clock += dt;

        UpdateHints();
        UpdateBell(dt);
        if (_picking) UpdateLock(dt);
    }

    private void UpdateHints()
    {
        if (_picking || _hintIndex >= Hints.Length || _clock < 1.0f) return;
        if (_player.GlobalPosition.X < Hints[_hintIndex].X) return;
        _hud.ShowHint(Hints[_hintIndex].Key, 8f);
        _hintIndex++;
    }

    private void UpdateBell(float dt)
    {
        if (_bellLeft > 0f)
        {
            _bellLeft -= dt;
            // Swell in, hold, die away.
            float t = 1f - _bellLeft / BellSeconds;
            NoiseBus.AmbientMask = Mathf.Clamp(Mathf.Min(t * 4f, (1f - t) * 3f), 0f, 1f);
            if (_bellLeft <= 0f)
            {
                NoiseBus.AmbientMask = 0f;
                _hud.SetBell(false);
            }
            return;
        }

        _bellTimer -= dt;
        if (_bellTimer > 0f) return;
        _bellTimer = BellEvery;
        _bellLeft = BellSeconds;
        _hud.SetBell(true);
    }

    private void OnDoorUsed(Node3D who)
    {
        if (_picking || _over) return;
        _picking = true;
        _pickNoise = 0f;
        _rig.EnterFirstPerson(_door.LookTarget, scripted: true);
        _hud.SetObjective("PROLOGUE_OBJ_PICK");
        _hud.ShowHint("HINT_LOCKPICK", 0f);
        _hud.SetProgress(_pickProgress);
    }

    private void UpdateLock(float dt)
    {
        if (Input.IsActionJustPressed(InputActions.MoveBack))
        {
            _picking = false;
            _rig.ExitFirstPerson();
            _hud.SetProgress(-1f);
            _hud.ShowHint("", 0f);
            _hud.SetObjective("PROLOGUE_OBJ_REACH");
            return;
        }

        if (!Input.IsActionPressed(InputActions.Interact)) return;

        _pickProgress += dt / _player.Profile.LockpickSeconds;
        _hud.SetProgress(_pickProgress);

        _pickNoise -= dt;
        if (_pickNoise <= 0f)
        {
            _pickNoise = LockNoiseEvery;
            NoiseBus.Emit(_player.GlobalPosition, LockNoiseRadius * _player.Profile.NoiseMultiplier, NoiseKind.Tool, _player);
        }

        if (_pickProgress >= 1f) Finish();
    }

    private void Finish()
    {
        _over = true;
        _picking = false;
        _door.Enabled = false;
        NoiseBus.AmbientMask = 0f;
        _hud.SetProgress(-1f);
        _hud.ShowHint("", 0f);
        _hud.ShowBanner("PROLOGUE_END_TITLE", "PROLOGUE_END_BODY");
        GetTree().CreateTimer(7.0).Timeout += () => SceneRouter.Instance.GoTo(SceneRouter.MainMenu, 1.2f);
    }

    private void OnAlarm(Watchman watchman)
    {
        if (_over) return;
        _over = true;
        _picking = false;
        if (_rig.FirstPerson) _rig.ExitFirstPerson();
        _player.InputLocked = true;
        NoiseBus.AmbientMask = 0f;
        _hud.SetProgress(-1f);
        _hud.ShowHint("", 0f);
        _hud.ShowBanner("CAUGHT_TITLE", "CAUGHT_BODY");
        GetTree().CreateTimer(4.0).Timeout += () => SceneRouter.Instance.GoTo(SceneRouter.Prologue);
    }

    private void OnViewChanged(bool firstPerson) => _hud.SetFirstPerson(firstPerson);
}
