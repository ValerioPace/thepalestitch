using Godot;
using ThePaleStitch.Core;

namespace ThePaleStitch.Intro;

/// <summary>
/// First screen: a line of warning on black, then the opening film the first
/// time the game is run, the main menu every time after.
/// </summary>
public partial class Boot : Control
{
    [Export] public float Seconds = 3.5f;

    private bool _leaving;

    public override void _Ready()
    {
        var notice = GetNode<Label>("%Notice");
        notice.Modulate = new Color(1, 1, 1, 0);
        CreateTween().TweenProperty(notice, "modulate:a", 1f, 1.2f);
        // A child timer dies with the scene: no call into a freed Boot (FIX-014).
        var timer = new Timer { WaitTime = Seconds, OneShot = true };
        timer.Timeout += Leave;
        AddChild(timer);
        timer.Start();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed(InputActions.Skip)) Leave();
    }

    private void Leave()
    {
        if (_leaving) return;
        _leaving = true;
        SceneRouter.Instance.GoTo(GameState.Instance.IntroSeen ? SceneRouter.MainMenu : SceneRouter.Intro);
    }
}
