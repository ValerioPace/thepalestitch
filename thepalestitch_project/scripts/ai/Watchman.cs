using System;
using System.Collections.Generic;
using Godot;
using ThePaleStitch.Core;
using ThePaleStitch.Player;
using ThePaleStitch.Rules;
using ThePaleStitch.Stealth;

namespace ThePaleStitch.AI;

/// <summary>
/// A man of the watch with a lantern. Walks a round, turns towards what he
/// hears, and grows surer of what he sees the better lit it is.
/// First pass of the guard brain: patrol, investigate, raise the alarm.
/// </summary>
public partial class Watchman : CharacterBody3D
{
    public const string Group = "guards";

    public enum State
    {
        Patrol,
        Investigate,
        Alert,
    }

    /// <summary>A Node3D whose Marker3D children are the round, walked in order.</summary>
    [Export] public NodePath PatrolPath;
    [Export] public float PatrolSpeed = 1.5f;
    [Export] public float InvestigateSpeed = 2.4f;
    [Export] public float WaitAtPoint = 1.5f;
    [Export] public float LookAroundTime = 3.5f;

    [ExportGroup("Senses")]
    [Export] public float ViewRange = 14f;
    [Export] public float ViewHalfAngleDegrees = 38f;
    /// <summary>Suspicion per second when a fully lit player stands right in front of him.</summary>
    [Export] public float VisionGain = 1.6f;
    [Export] public float Forget = 0.22f;
    /// <summary>Below this visibility he sees nothing unless he walks into it.</summary>
    [Export] public float MinVisibility = 0.08f;
    [Export] public float BumpDistance = 1.3f;
    /// <summary>Suspicion at which he leaves his round to go and look.</summary>
    [Export] public float InvestigateAt = 0.4f;

    /// <summary>0 = at ease, 1 = alarm.</summary>
    public float Suspicion { get; private set; }

    /// <summary>Result of the last sight test, 0..1 (debug overlay).</summary>
    public float LastSight { get; private set; }

    /// <summary>Where the guard sees and hears from.</summary>
    public Vector3 EyesPosition => _eyes.GlobalPosition;

    public State Current { get; private set; } = State.Patrol;

    /// <summary>Raised once, when suspicion reaches 1.</summary>
    public event Action<Watchman> RaisedAlarm;

    private readonly List<Vector3> _points = new();
    private NavigationAgent3D _agent;
    private Node3D _eyes;
    private Label3D _mark;
    private PlayerController _player;
    private int _pointIndex;
    private float _wait;
    private Vector3 _interest;
    private float _gravity;

    public override void _EnterTree() => NoiseBus.Emitted += OnNoise;
    public override void _ExitTree() => NoiseBus.Emitted -= OnNoise;

    public override void _Ready()
    {
        AddToGroup(Group);
        _agent = GetNodeOrNull<NavigationAgent3D>("NavAgent");
        _eyes = GetNode<Node3D>("Eyes");
        _mark = GetNodeOrNull<Label3D>("Mark");
        _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

        if (GetNodeOrNull(PatrolPath) is { } path)
            foreach (Node child in path.GetChildren())
                if (child is Node3D point) _points.Add(point.GlobalPosition);
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        _player ??= GetTree().GetFirstNodeInGroup(PlayerController.Group) as PlayerController;

        if (Current != State.Alert) Sense(dt);

        Vector3 move = Vector3.Zero;
        switch (Current)
        {
            case State.Patrol:
                move = Patrol(dt);
                break;
            case State.Investigate:
                move = Investigate(dt);
                break;
            case State.Alert:
                if (_player != null) Face(_player.GlobalPosition - GlobalPosition, dt * 2f);
                break;
        }

        Vector3 velocity = Velocity;
        velocity.X = move.X;
        velocity.Z = move.Z;
        velocity.Y = IsOnFloor() ? 0f : velocity.Y - _gravity * dt;
        Velocity = velocity;
        MoveAndSlide();

        if (_mark != null)
        {
            _mark.Text = Current == State.Alert ? "!" : Suspicion > 0.25f ? "?" : "";
            _mark.Modulate = Current == State.Alert ? new Color("b3261e") : new Color("d9d2c0");
        }
    }

    private void Sense(float dt)
    {
        float seen = _player != null ? Sight(_player) : 0f;
        LastSight = seen;
        if (seen > 0f)
        {
            Suspicion = Mathf.Min(1f, Suspicion + seen * VisionGain * dt);
            _interest = _player.GlobalPosition;
            if (Suspicion >= InvestigateAt && Current == State.Patrol) StartInvestigating(_interest);
        }
        else
        {
            Suspicion = Mathf.Max(0f, Suspicion - Forget * dt);
        }

        if (Suspicion >= 1f)
        {
            Current = State.Alert;
            RaisedAlarm?.Invoke(this);
        }
    }

    /// <summary>0 when the player cannot be seen, up to 1 when lit and close.</summary>
    private float Sight(PlayerController player)
    {
        Vector3 eyes = _eyes.GlobalPosition;
        Vector3 target = player.GlobalPosition + Vector3.Up * 1.0f;
        Vector3 to = target - eyes;
        float distance = to.Length();
        if (distance > ViewRange) return 0f;

        bool bump = distance < BumpDistance;
        if (!bump)
        {
            if (player.Visibility < MinVisibility) return 0f;
            Vector3 forward = -GlobalBasis.Z;
            Vector3 flat = new Vector3(to.X, 0f, to.Z).Normalized();
            if (Mathf.RadToDeg(forward.AngleTo(flat)) > ViewHalfAngleDegrees) return 0f;
        }

        var exclude = new Godot.Collections.Array<Rid> { GetRid(), player.GetRid() };
        var query = PhysicsRayQueryParameters3D.Create(eyes, target, PhysicsLayers.OccluderMask, exclude);
        if (GetWorld3D().DirectSpaceState.IntersectRay(query).Count > 0) return 0f;

        if (bump) return 1f;
        float nearness = 0.25f + 0.75f * (1f - distance / ViewRange);
        return player.Visibility * nearness;
    }

    private void OnNoise(NoiseEvent noise)
    {
        if (noise.Source == this || Current == State.Alert || !IsInsideTree()) return;
        Vector3 ears = _eyes.GlobalPosition;
        if (!NoiseBus.CanHear(GetWorld3D().DirectSpaceState, noise, ears)) return;

        // A noise near the edge of hearing is a doubt; one close by is nearly a
        // certainty. Hearing never lowers suspicion and never raises the alarm.
        Suspicion = StealthRules.SuspicionAfterHearing(Suspicion, noise.Origin.DistanceTo(ears), noise.Radius, InvestigateAt);
        StartInvestigating(noise.Origin);
    }

    private void StartInvestigating(Vector3 where)
    {
        _interest = where;
        _wait = LookAroundTime;
        Current = State.Investigate;
    }

    private Vector3 Patrol(float dt)
    {
        if (_points.Count == 0) return Vector3.Zero;
        Vector3 target = _points[_pointIndex];
        if (Flat(target - GlobalPosition).Length() < 0.4f)
        {
            _wait -= dt;
            if (_wait <= 0f)
            {
                _pointIndex = (_pointIndex + 1) % _points.Count;
                _wait = WaitAtPoint;
            }
            return Vector3.Zero;
        }
        return StepTowards(target, PatrolSpeed, dt);
    }

    private Vector3 Investigate(float dt)
    {
        if (Flat(_interest - GlobalPosition).Length() > 1.2f)
            return StepTowards(_interest, InvestigateSpeed, dt);

        // Arrived: stand and sweep the lantern about.
        RotateY(Mathf.DegToRad(70f) * dt * Mathf.Sign(Mathf.Sin(_wait * 1.6f)));
        _wait -= dt;
        if (_wait <= 0f && Suspicion < InvestigateAt)
        {
            Current = State.Patrol;
            _wait = WaitAtPoint;
        }
        return Vector3.Zero;
    }

    private Vector3 StepTowards(Vector3 target, float speed, float dt)
    {
        Vector3 next = target;
        if (_agent != null && NavigationServer3D.MapGetIterationId(_agent.GetNavigationMap()) > 0)
        {
            _agent.TargetPosition = target;
            if (!_agent.IsNavigationFinished()) next = _agent.GetNextPathPosition();
        }

        Vector3 direction = Flat(next - GlobalPosition);
        if (direction.Length() < 0.05f) direction = Flat(target - GlobalPosition);
        if (direction.Length() < 0.05f) return Vector3.Zero;
        direction = direction.Normalized();
        Face(direction, dt);
        return direction * speed;
    }

    private void Face(Vector3 direction, float dt)
    {
        direction = Flat(direction);
        if (direction.LengthSquared() < 0.0001f) return;
        float yaw = Mathf.Atan2(-direction.X, -direction.Z);
        Rotation = new Vector3(0f, Mathf.LerpAngle(Rotation.Y, yaw, 1f - Mathf.Exp(-6f * dt)), 0f);
    }

    private static Vector3 Flat(Vector3 v) => new(v.X, 0f, v.Z);
}
