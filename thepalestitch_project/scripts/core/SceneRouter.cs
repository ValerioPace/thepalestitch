using System;
using Godot;

namespace ThePaleStitch.Core;

/// <summary>
/// Autoload. Changes scene through a fade to black so every transition in the
/// game has the same slow, heavy cut.
/// </summary>
public partial class SceneRouter : CanvasLayer
{
    public const string Intro = "res://scenes/intro/IntroCinematic.tscn";
    public const string MainMenu = "res://scenes/menu/MainMenu.tscn";
    public const string CharacterCreation = "res://scenes/character/CharacterCreation.tscn";
    public const string Prologue = "res://thepalestitch_prologue.tscn";

    public static SceneRouter Instance { get; private set; }

    private ColorRect _fade;
    private bool _busy;
    private string _queuedPath;
    private float _queuedFade;

    /// <summary>True while a transition is running (used by the tests).</summary>
    public bool Busy => _busy;

    public override void _EnterTree() => Instance = this;

    public override void _Ready()
    {
        Layer = 100;
        ProcessMode = ProcessModeEnum.Always;

        _fade = new ColorRect
        {
            Name = "Fade",
            Color = Colors.Black,
            Modulate = new Color(1, 1, 1, 0),
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        _fade.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        AddChild(_fade);
    }

    /// <summary>
    /// Fade out, load <paramref name="scenePath"/>, fade back in. A request made
    /// during a transition is not lost: the last one is run when the current
    /// transition ends (FIX-008).
    /// </summary>
    public async void GoTo(string scenePath, float fadeSeconds = 0.7f)
    {
        if (_busy)
        {
            _queuedPath = scenePath;
            _queuedFade = fadeSeconds;
            return;
        }
        _busy = true;
        _fade.MouseFilter = Control.MouseFilterEnum.Stop;

        try
        {
            var fadeOut = CreateTween();
            fadeOut.TweenProperty(_fade, "modulate:a", 1.0f, fadeSeconds);
            await ToSignal(fadeOut, Tween.SignalName.Finished);

            GetTree().Paused = false;
            Mouse.SetMode(Input.MouseModeEnum.Visible);
            var error = GetTree().ChangeSceneToFile(scenePath);
            if (error != Error.Ok)
                GD.PushError($"SceneRouter: cannot load '{scenePath}' ({error}).");

            // Let the new scene run its _Ready before it becomes visible.
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

            var fadeIn = CreateTween();
            fadeIn.TweenProperty(_fade, "modulate:a", 0.0f, fadeSeconds);
            await ToSignal(fadeIn, Tween.SignalName.Finished);
        }
        catch (Exception e)
        {
            // async void: nobody else would ever see this exception.
            GD.PushError($"SceneRouter: transition to '{scenePath}' failed: {e}");
            _fade.Modulate = new Color(1, 1, 1, 0);
        }
        finally
        {
            _fade.MouseFilter = Control.MouseFilterEnum.Ignore;
            _busy = false;
        }

        if (_queuedPath != null)
        {
            string next = _queuedPath;
            _queuedPath = null;
            GoTo(next, _queuedFade);
        }
    }
}
