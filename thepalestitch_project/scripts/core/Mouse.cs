using Godot;

namespace ThePaleStitch.Core;

/// <summary>
/// Changes the mouse mode where there is a mouse to change. Without a display
/// (headless runs of the scenario bench, servers) the call is skipped instead
/// of filling the log with "not supported" errors.
/// </summary>
public static class Mouse
{
    public static void SetMode(Input.MouseModeEnum mode)
    {
        if (DisplayServer.GetName() == "headless") return;
        Input.MouseMode = mode;
    }
}
