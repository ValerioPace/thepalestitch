using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using ThePaleStitch.AI;
using ThePaleStitch.Player;
using ThePaleStitch.UI;

namespace ThePaleStitch.Dev;

/// <summary>One check made by a scenario.</summary>
public sealed record ScenarioCheck(string Name, bool Passed, string Detail);

/// <summary>
/// What a scenario can do: load a scene, let simulated time pass, press
/// actions, find the actors, record checks and measurements.
/// </summary>
public sealed class ScenarioContext
{
    private readonly Node _host;

    public ScenarioContext(Node host, string reportDir)
    {
        _host = host;
        ReportDir = reportDir;
    }

    public string ReportDir { get; }
    public List<ScenarioCheck> Checks { get; } = new();
    public List<string> Notes { get; } = new();

    public SceneTree Tree => _host.GetTree();
    public Node Scene => Tree.CurrentScene;
    public PlayerController Player => Tree.GetFirstNodeInGroup(PlayerController.Group) as PlayerController;
    public Watchman Guard => Tree.GetFirstNodeInGroup(Watchman.Group) as Watchman;
    public Hud Hud => Scene?.GetNodeOrNull<Hud>("Hud");

    public async Task Load(string path)
    {
        Input.ActionRelease("sneak");
        foreach (StringName action in InputMap.GetActions()) Input.ActionRelease(action);
        Tree.Paused = false;
        Tree.ChangeSceneToFile(path);
        await Frames(4);
    }

    /// <summary>Let <paramref name="seconds"/> of game time pass.</summary>
    public async Task Wait(double seconds) =>
        await _host.ToSignal(Tree.CreateTimer(seconds, processAlways: true), SceneTreeTimer.SignalName.Timeout);

    public async Task Frames(int count)
    {
        for (int i = 0; i < count; i++)
            await _host.ToSignal(Tree, SceneTree.SignalName.PhysicsFrame);
    }

    /// <summary>Press and release an action as a single key stroke, through the input pipeline.</summary>
    public async Task Tap(StringName action)
    {
        Input.ParseInputEvent(new InputEventAction { Action = action, Pressed = true });
        await Frames(2);
        Input.ParseInputEvent(new InputEventAction { Action = action, Pressed = false });
        await Frames(2);
    }

    public void Check(string name, bool passed, string detail = "") => Checks.Add(new ScenarioCheck(name, passed, detail));

    public void Note(string text) => Notes.Add(text);

    public static float Flat(Vector3 a, Vector3 b) => new Vector2(a.X - b.X, a.Z - b.Z).Length();
}
