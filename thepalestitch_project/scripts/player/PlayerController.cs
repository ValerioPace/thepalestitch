using System.Collections.Generic;
using Godot;
using ThePaleStitch.Character;
using ThePaleStitch.Core;
using ThePaleStitch.Stealth;
using ThePaleStitch.World;

namespace ThePaleStitch.Player;

/// <summary>
/// The cutthroat. Moves relative to the isometric camera, makes noise with
/// every step according to stance, floor and garb, and knows how visible he is.
///
/// Stance: creeping is a toggle (press the sneak key once to start, once to
/// stop); running is held, and on release the chosen stance comes back. While
/// the legs are locked by an action (a lock, first person) the stance is
/// <see cref="Stance.Busy"/>, which does not touch the chosen one.
/// </summary>
public partial class PlayerController : CharacterBody3D
{
    public const string Group = "player";

    public enum Stance
    {
        Sneak,
        Walk,
        Run,
        /// <summary>Legs locked by an action in progress (lock, first person).</summary>
        Busy,
    }

    [ExportGroup("Speed (m/s)")]
    [Export] public float SneakSpeed = 1.6f;
    [Export] public float WalkSpeed = 3.2f;
    [Export] public float RunSpeed = 5.6f;
    [Export] public float Acceleration = 14f;

    [ExportGroup("Footstep noise radius (m)")]
    [Export] public float SneakNoise = 1.5f;
    [Export] public float WalkNoise = 4.5f;
    [Export] public float RunNoise = 9.0f;

    [ExportGroup("Visibility by stance")]
    [Export] public float SneakVisibility = 0.75f;
    [Export] public float WalkVisibility = 1.0f;
    [Export] public float RunVisibility = 1.15f;
    /// <summary>During an action in first person. Neutral by decision D-03.</summary>
    [Export] public float BusyVisibility = 1.0f;

    [ExportGroup("Seconds between footsteps")]
    [Export] public float SneakStep = 0.62f;
    [Export] public float WalkStep = 0.45f;
    [Export] public float RunStep = 0.30f;

    /// <summary>Set by cutscenes and first-person actions to freeze the legs.</summary>
    public bool InputLocked { get; set; }

    public Stance CurrentStance { get; private set; } = Stance.Walk;

    /// <summary>The creeping toggle chosen by the player. Survives runs and actions.</summary>
    public bool SneakToggled { get; private set; }

    /// <summary>0 = unseen, 1 = fully lit. Light, garb, trade and stance together.</summary>
    public float Visibility { get; private set; }

    /// <summary>0..1, how loud the last footsteps were. Decays; drives the HUD.</summary>
    public float NoiseLevel { get; private set; }

    public SurfaceType CurrentSurface => _surfaces.Count > 0 ? TopSurface().Surface : SurfaceType.Cobble;

    /// <summary>Radius of the last footstep as emitted (after the ambient mask), metres.</summary>
    public float LastNoiseRadius { get; private set; }

    public LightSensor Sensor => _sensor;

    /// <summary>The interactable in reach, or null.</summary>
    public Interactable Focus { get; private set; }

    public CharacterProfile Profile { get; private set; }
    public Node3D Head { get; private set; }
    public Camera3D FirstPersonCamera { get; private set; }
    public Node3D Visual { get; private set; }

    private const float NoiseForFullMeter = 12f;

    private readonly List<SurfaceZone> _surfaces = new();
    private LightSensor _sensor;
    private Area3D _probe;
    private float _stepTimer;
    private float _gravity;

    public override void _Ready()
    {
        AddToGroup(Group);
        Head = GetNode<Node3D>("Head");
        // The head turns with the mouse, between physics steps: no interpolation lag.
        Head.PhysicsInterpolationMode = PhysicsInterpolationModeEnum.Off;
        FirstPersonCamera = GetNode<Camera3D>("Head/FirstPersonCamera");
        Visual = GetNode<Node3D>("Visual");
        _sensor = GetNode<LightSensor>("LightSensor");
        _probe = GetNode<Area3D>("InteractProbe");
        _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

        Profile = GameState.Instance?.Profile ?? CharacterProfile.Default();
        if (Visual is Mannequin mannequin)
            mannequin.Apply(Profile.Dye);
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;

        Vector2 input = InputLocked
            ? Vector2.Zero
            : Input.GetVector(InputActions.MoveLeft, InputActions.MoveRight, InputActions.MoveForward, InputActions.MoveBack);
        float yaw = CameraRig.Current?.Yaw ?? Mathf.DegToRad(45f);
        Vector3 direction = new Vector3(input.X, 0f, input.Y).Rotated(Vector3.Up, yaw);

        CurrentStance = InputLocked ? Stance.Busy
            : Input.IsActionPressed(InputActions.Run) ? Stance.Run
            : SneakToggled ? Stance.Sneak
            : Stance.Walk;

        Vector3 wanted = direction * SpeedOf(CurrentStance);
        Vector3 velocity = Velocity;
        velocity.X = Mathf.MoveToward(velocity.X, wanted.X, Acceleration * dt);
        velocity.Z = Mathf.MoveToward(velocity.Z, wanted.Z, Acceleration * dt);
        velocity.Y = IsOnFloor() ? 0f : velocity.Y - _gravity * dt;
        Velocity = velocity;
        MoveAndSlide();

        bool moving = new Vector2(Velocity.X, Velocity.Z).Length() > 0.3f;
        if (moving)
        {
            float facing = Mathf.Atan2(-Velocity.X, -Velocity.Z);
            Visual.Rotation = new Vector3(0f, Mathf.LerpAngle(Visual.Rotation.Y, facing, 1f - Mathf.Exp(-12f * dt)), 0f);
        }
        float crouch = CurrentStance == Stance.Sneak ? 0.82f : 1f;
        Visual.Scale = Visual.Scale.Lerp(new Vector3(1f, crouch, 1f), 1f - Mathf.Exp(-10f * dt));

        UpdateFootsteps(moving, dt);
        UpdateVisibility();
        UpdateFocus();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (InputLocked) return;
        if (@event.IsActionPressed(InputActions.Sneak))
        {
            SneakToggled = !SneakToggled;
            GetViewport().SetInputAsHandled();
            return;
        }
        if (Focus != null && @event.IsActionPressed(InputActions.Interact))
        {
            Focus.Interact(this);
            GetViewport().SetInputAsHandled();
        }
    }

    public void EnterSurface(SurfaceZone zone)
    {
        if (!_surfaces.Contains(zone)) _surfaces.Add(zone);
    }

    public void ExitSurface(SurfaceZone zone) => _surfaces.Remove(zone);

    /// <summary>Radius of a footstep right now, before the ambient mask.</summary>
    public float FootstepRadius()
    {
        float surface = _surfaces.Count > 0 ? TopSurface().NoiseMultiplier : SurfaceZone.Loudness(SurfaceType.Cobble);
        return NoiseOf(CurrentStance) * surface * Profile.NoiseMultiplier;
    }

    private void UpdateFootsteps(bool moving, float dt)
    {
        NoiseLevel = Mathf.MoveToward(NoiseLevel, 0f, dt * 1.2f);
        if (!moving || !IsOnFloor())
        {
            _stepTimer = 0f;
            return;
        }

        _stepTimer -= dt;
        if (_stepTimer > 0f) return;
        _stepTimer = StepOf(CurrentStance);

        float heard = NoiseBus.Emit(GlobalPosition, FootstepRadius(), NoiseKind.Footstep, this);
        LastNoiseRadius = heard;
        NoiseLevel = Mathf.Max(NoiseLevel, Mathf.Clamp(heard / NoiseForFullMeter, 0f, 1f));
    }

    private void UpdateVisibility()
    {
        float stance = CurrentStance switch
        {
            Stance.Sneak => SneakVisibility,
            Stance.Run => RunVisibility,
            Stance.Busy => BusyVisibility,
            _ => WalkVisibility,
        };
        Visibility = Mathf.Clamp(_sensor.Exposure * Profile.VisibilityMultiplier * stance, 0f, 1f);
    }

    private void UpdateFocus()
    {
        Interactable best = null;
        float bestDistance = float.MaxValue;
        foreach (Area3D area in _probe.GetOverlappingAreas())
        {
            if (area is not Interactable { Enabled: true } candidate) continue;
            float distance = candidate.GlobalPosition.DistanceSquaredTo(GlobalPosition);
            if (distance < bestDistance)
            {
                best = candidate;
                bestDistance = distance;
            }
        }
        Focus = best;
    }

    private SurfaceZone TopSurface()
    {
        SurfaceZone top = _surfaces[0];
        foreach (SurfaceZone zone in _surfaces)
            if (zone.Priority > top.Priority) top = zone;
        return top;
    }

    private float SpeedOf(Stance stance) => stance switch
    {
        Stance.Sneak => SneakSpeed,
        Stance.Run => RunSpeed,
        _ => WalkSpeed,
    };

    private float NoiseOf(Stance stance) => stance switch
    {
        Stance.Sneak => SneakNoise,
        Stance.Run => RunNoise,
        _ => WalkNoise,
    };

    private float StepOf(Stance stance) => stance switch
    {
        Stance.Sneak => SneakStep,
        Stance.Run => RunStep,
        _ => WalkStep,
    };
}
