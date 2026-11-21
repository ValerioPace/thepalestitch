using System.Collections.Generic;
using Godot;
using ThePaleStitch.Core;
using ThePaleStitch.UI;

namespace ThePaleStitch.Intro;

/// <summary>
/// The opening film, played in engine. Runs the <see cref="IntroShot"/> children
/// of the "Shots" node in order, each fading in from black and out again, with
/// the narration as subtitles; then sews the title and goes to the main menu.
/// </summary>
public partial class IntroCinematic : Node3D
{
    [Export] public float FadeSeconds = 1.2f;
    /// <summary>Seconds into a shot before its narration line appears.</summary>
    [Export] public float SubtitleDelay = 1.5f;
    [Export] public float CardSeconds = 5f;
    [Export] public float TitleSeconds = 8f;

    private readonly List<IntroShot> _shots = new();
    private Camera3D _camera;
    private ColorRect _black;
    private Label _subtitle;
    private Label _card;
    private Label _title;
    private Label _skipHint;
    private StitchLine _stitch;

    private int _index;
    private float _time;
    private bool _titlePhase;
    private bool _leaving;

    public override void _Ready()
    {
        _camera = GetNode<Camera3D>("Camera");
        // The film camera is moved every rendered frame: no physics interpolation.
        _camera.PhysicsInterpolationMode = PhysicsInterpolationModeEnum.Off;
        _black = GetNode<ColorRect>("%Black");
        _subtitle = GetNode<Label>("%Subtitle");
        _card = GetNode<Label>("%Card");
        _title = GetNode<Label>("%Title");
        _skipHint = GetNode<Label>("%SkipHint");
        _stitch = GetNode<StitchLine>("%Stitch");

        foreach (Node child in GetNode("Shots").GetChildren())
            if (child is IntroShot shot) _shots.Add(shot);

        _title.Modulate = new Color(1, 1, 1, 0);
        _stitch.Progress = 0f;
        _stitch.Visible = false;
        _camera.MakeCurrent();
        StartShot(0);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed(InputActions.Skip)) Leave();
    }

    public override void _Process(double delta)
    {
        if (_leaving) return;
        _time += (float)delta;
        _skipHint.Modulate = new Color(1, 1, 1, Mathf.Clamp(3f - _time * 0.5f, 0f, 0.6f));

        if (_titlePhase)
        {
            UpdateTitle();
            return;
        }

        IntroShot shot = _shots[_index];
        float t = Mathf.Clamp(_time / shot.Duration, 0f, 1f);
        float eased = Mathf.SmoothStep(0f, 1f, t);
        _camera.GlobalPosition = shot.CameraAt(eased);
        _camera.LookAt(shot.TargetAt(eased), Vector3.Up);

        float fromEnd = shot.Duration - _time;
        _black.Color = new Color(0, 0, 0, 1f - Window(_time, fromEnd, FadeSeconds));
        _subtitle.Modulate = new Color(1, 1, 1, Window(_time - SubtitleDelay, fromEnd - FadeSeconds, 0.8f));
        _card.Modulate = new Color(1, 1, 1, Window(_time - 0.8f, CardSeconds - _time, 0.8f));

        if (_time < shot.Duration) return;
        if (_index + 1 < _shots.Count) StartShot(_index + 1);
        else StartTitle();
    }

    private void StartShot(int index)
    {
        if (_shots.Count == 0)
        {
            StartTitle();
            return;
        }
        _index = index;
        _time = 0f;
        IntroShot shot = _shots[index];
        _camera.Fov = shot.Fov;
        _subtitle.Text = shot.SubtitleKey;
        _card.Text = shot.CardKey;
        _subtitle.Modulate = new Color(1, 1, 1, 0);
        _card.Modulate = new Color(1, 1, 1, 0);
        _black.Color = Colors.Black;
    }

    private void StartTitle()
    {
        _titlePhase = true;
        _time = 0f;
        _black.Color = Colors.Black;
        _subtitle.Text = "";
        _card.Text = "";
        _stitch.Visible = true;
    }

    private void UpdateTitle()
    {
        _stitch.Progress = Mathf.Clamp((_time - 0.5f) / 3f, 0f, 1f);
        _title.Modulate = new Color(1, 1, 1, Mathf.Clamp((_time - 2.5f) / 2f, 0f, 1f));
        if (_time >= TitleSeconds) Leave();
    }

    private void Leave()
    {
        if (_leaving) return;
        _leaving = true;
        GameState.Instance.MarkIntroSeen();
        SceneRouter.Instance.GoTo(SceneRouter.MainMenu, 1.2f);
    }

    /// <summary>1 inside a time window, ramping over <paramref name="ramp"/> seconds at both ends.</summary>
    private static float Window(float sinceStart, float untilEnd, float ramp) =>
        Mathf.Clamp(Mathf.Min(sinceStart, untilEnd) / ramp, 0f, 1f);
}
