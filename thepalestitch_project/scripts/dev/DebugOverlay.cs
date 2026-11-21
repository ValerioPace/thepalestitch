using System.Text;
using Godot;
using ThePaleStitch.AI;
using ThePaleStitch.Core;
using ThePaleStitch.Player;
using ThePaleStitch.Stealth;

namespace ThePaleStitch.Dev;

/// <summary>
/// Stealth debug overlay (CR-020), toggled with F3 in debug builds only.
///
/// In the world: each guard's view cone and "bump" circle, coloured by state;
/// his line of sight to the player (green clear, red blocked); every light ray
/// the player's light sensor tested (yellow lights him, dark red blocked).
/// On screen: the numbers behind it all.
/// </summary>
public partial class DebugOverlay : Node
{
    private static readonly StringName Toggle = "debug_overlay";

    private MeshInstance3D _lines;
    private ImmediateMesh _mesh;
    private StandardMaterial3D _material;
    private CanvasLayer _layer;
    private Label _text;
    private bool _on;

    public bool On => _on;

    public override void _Ready()
    {
        if (!OS.IsDebugBuild())
        {
            QueueFree();
            return;
        }
        ProcessMode = ProcessModeEnum.Always;

        _mesh = new ImmediateMesh();
        _material = new StandardMaterial3D
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            VertexColorUseAsAlbedo = true,
            NoDepthTest = true,
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
        };
        _lines = new MeshInstance3D
        {
            Name = "Lines",
            Mesh = _mesh,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            PhysicsInterpolationMode = PhysicsInterpolationModeEnum.Off,
            Visible = false,
        };
        AddChild(_lines);

        _layer = new CanvasLayer { Layer = 90, Visible = false };
        var panel = new PanelContainer { Position = new Vector2(24, 110) };
        _text = new Label();
        _text.AddThemeFontSizeOverride("font_size", 18);
        _text.AutoTranslateMode = AutoTranslateModeEnum.Disabled;
        panel.AddChild(_text);
        _layer.AddChild(panel);
        AddChild(_layer);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!@event.IsActionPressed(Toggle)) return;
        SetOn(!_on);
        GetViewport().SetInputAsHandled();
    }

    public void SetOn(bool on)
    {
        _on = on;
        _lines.Visible = on;
        _layer.Visible = on;
        if (GetTree().GetFirstNodeInGroup(PlayerController.Group) is PlayerController player && player.Sensor != null)
            player.Sensor.RecordRays = on;
        if (!on) _mesh.ClearSurfaces();
    }

    public override void _Process(double delta)
    {
        if (!_on) return;
        var player = GetTree().GetFirstNodeInGroup(PlayerController.Group) as PlayerController;
        if (player?.Sensor != null) player.Sensor.RecordRays = true;

        _mesh.ClearSurfaces();
        var text = new StringBuilder();
        text.AppendLine($"F3 debug · {Engine.GetFramesPerSecond()} fps · campana {NoiseBus.AmbientMask:0.00}");
        bool begun = false;

        void Line(Vector3 a, Vector3 b, Color color)
        {
            if (!begun)
            {
                _mesh.SurfaceBegin(Mesh.PrimitiveType.Lines, _material);
                begun = true;
            }
            _mesh.SurfaceSetColor(color);
            _mesh.SurfaceAddVertex(a);
            _mesh.SurfaceSetColor(color);
            _mesh.SurfaceAddVertex(b);
        }

        if (player != null)
        {
            text.AppendLine($"Giocatore  {player.CurrentStance}{(player.SneakToggled ? " (furtivo fissato)" : "")}  superficie {player.CurrentSurface}");
            text.AppendLine($"  esposizione {player.Sensor.Exposure:0.00} × personaggio {player.Profile.VisibilityMultiplier:0.00} → visibilità {player.Visibility:0.00}");
            text.AppendLine($"  ultimo passo {player.LastNoiseRadius:0.0} m  (personaggio ×{player.Profile.NoiseMultiplier:0.00})");
            foreach (LightSensor.LightRay ray in player.Sensor.Rays)
                Line(ray.Point, ray.Light, ray.Blocked ? new Color(0.5f, 0.1f, 0.1f, 0.6f) : new Color(1f, 0.85f, 0.3f, 0.35f + ray.Contribution));
        }

        foreach (Node node in GetTree().GetNodesInGroup(Watchman.Group))
        {
            if (node is not Watchman guard) continue;
            Color state = guard.Current switch
            {
                Watchman.State.Alert => new Color(0.9f, 0.15f, 0.1f),
                Watchman.State.Investigate => new Color(1f, 0.75f, 0.2f),
                _ => new Color(0.85f, 0.85f, 0.85f),
            };
            Vector3 eyes = guard.EyesPosition;
            Vector3 ground = new(eyes.X, guard.GlobalPosition.Y + 0.05f, eyes.Z);
            Vector3 forward = -guard.GlobalBasis.Z;
            forward = new Vector3(forward.X, 0, forward.Z).Normalized();
            float half = Mathf.DegToRad(guard.ViewHalfAngleDegrees);

            // Cone: two edges and an arc at full range.
            Vector3 left = forward.Rotated(Vector3.Up, half) * guard.ViewRange;
            Vector3 right = forward.Rotated(Vector3.Up, -half) * guard.ViewRange;
            Line(ground, ground + left, state);
            Line(ground, ground + right, state);
            const int arc = 16;
            for (int i = 0; i < arc; i++)
            {
                Vector3 a = forward.Rotated(Vector3.Up, Mathf.Lerp(half, -half, i / (float)arc)) * guard.ViewRange;
                Vector3 b = forward.Rotated(Vector3.Up, Mathf.Lerp(half, -half, (i + 1) / (float)arc)) * guard.ViewRange;
                Line(ground + a, ground + b, state);
            }
            // Bump circle.
            for (int i = 0; i < arc; i++)
            {
                Vector3 a = Vector3.Forward.Rotated(Vector3.Up, Mathf.Tau * i / arc) * guard.BumpDistance;
                Vector3 b = Vector3.Forward.Rotated(Vector3.Up, Mathf.Tau * (i + 1) / arc) * guard.BumpDistance;
                Line(ground + a, ground + b, state with { A = 0.5f });
            }

            float distance = 0f;
            if (player != null)
            {
                Vector3 target = player.GlobalPosition + Vector3.Up;
                distance = eyes.DistanceTo(target);
                var query = PhysicsRayQueryParameters3D.Create(eyes, target, PhysicsLayers.OccluderMask,
                    new Godot.Collections.Array<Rid> { guard.GetRid(), player.GetRid() });
                bool blocked = guard.GetWorld3D().DirectSpaceState.IntersectRay(query).Count > 0;
                Line(eyes, target, blocked ? new Color(0.6f, 0.1f, 0.1f, 0.5f) : new Color(0.3f, 0.9f, 0.4f, 0.8f));
            }
            text.AppendLine($"{guard.Name}  {guard.Current}  sospetto {guard.Suspicion:0.00}  vista {guard.LastSight:0.00}  distanza {distance:0.0} m");
        }

        if (begun) _mesh.SurfaceEnd();
        _text.Text = text.ToString().TrimEnd();
    }
}
