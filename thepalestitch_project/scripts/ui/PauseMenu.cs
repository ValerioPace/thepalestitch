using Godot;
using ThePaleStitch.Core;

namespace ThePaleStitch.UI;

/// <summary>Pause overlay. Lives inside the HUD scene and keeps running while the tree is paused.</summary>
public partial class PauseMenu : Control
{
    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        Visible = false;
        GetNode<Button>("%Resume").Pressed += () => SetPaused(false);
        GetNode<Button>("%ToMenu").Pressed += () => SceneRouter.Instance.GoTo(SceneRouter.MainMenu);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!@event.IsActionPressed(InputActions.Pause)) return;
        SetPaused(!Visible);
        GetViewport().SetInputAsHandled();
    }

    private Input.MouseModeEnum _mouseBefore;

    private void SetPaused(bool paused)
    {
        Visible = paused;
        GetTree().Paused = paused;
        if (paused)
        {
            _mouseBefore = Input.MouseMode;
            Mouse.SetMode(Input.MouseModeEnum.Visible);
            GetNode<Button>("%Resume").GrabFocus();
        }
        else
        {
            Mouse.SetMode(_mouseBefore);
        }
    }
}
