using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace ThePaleStitch.Dev;

/// <summary>
/// Entry point of the gameplay scenario bench (CR-011). Run headless:
///
/// <code>
/// godot --headless --fixed-fps 144 --path thepalestitch_project res://dev/ScenarioRunner.tscn -- [--scenario=name] [--report=dir]
/// </code>
///
/// <c>--fixed-fps</c> makes simulated time run as fast as the machine allows.
/// 144 draws 144 frames per simulated second over 60 physics steps, like a
/// fast monitor, which is what the smoothness check (FIX-002) needs. Writes report.md in the report
/// folder and quits with exit code 0 when every check passed, 1 otherwise.
/// </summary>
public partial class ScenarioRunner : Node
{
    public override void _Ready()
    {
        // The runner must outlive the scene changes the scenarios make, so the
        // work is done by a node parked under the root, not by this scene.
        var host = new Node { Name = "ScenarioHost", ProcessMode = ProcessModeEnum.Always };
        GetTree().Root.CallDeferred(Node.MethodName.AddChild, host);
        host.Ready += () => _ = Run(host);
    }

    private static async Task Run(Node host)
    {
        string filter = Arg("scenario");
        string reportDir = Arg("report") ?? ProjectSettings.GlobalizePath("res://").PathJoin("../verify-report").SimplifyPath();
        DirAccess.MakeDirRecursiveAbsolute(reportDir);

        var report = new StringBuilder();
        report.AppendLine("# Scenari di gioco").AppendLine();
        report.AppendLine($"Eseguiti il {DateTime.Now:yyyy-MM-dd HH:mm} — Godot {Engine.GetVersionInfo()["string"]}").AppendLine();
        report.AppendLine("| Scenario | Copre | Controllo | Esito | Dettaglio |");
        report.AppendLine("|---|---|---|---|---|");

        int failed = 0, passed = 0;
        foreach (Scenarios.Entry scenario in Scenarios.All.Where(s => filter == null || s.Name.Contains(filter)))
        {
            var context = new ScenarioContext(host, reportDir);
            GD.Print($"--- scenario {scenario.Name}");
            try
            {
                await scenario.Run(context);
            }
            catch (Exception e)
            {
                context.Check("runs without exceptions", false, e.GetType().Name + ": " + e.Message);
            }

            foreach (ScenarioCheck check in context.Checks)
            {
                if (check.Passed) passed++; else failed++;
                GD.Print($"  {(check.Passed ? "PASS" : "FAIL")}  {check.Name}  {check.Detail}");
                report.AppendLine($"| {scenario.Name} | {scenario.Covers} | {check.Name} | {(check.Passed ? "✅" : "❌")} | {check.Detail.Replace("|", "/")} |");
            }
            foreach (string note in context.Notes) GD.Print($"  note: {note}");
        }

        report.AppendLine().AppendLine($"**{passed} controlli superati, {failed} falliti.**");
        string path = reportDir.PathJoin("scenarios.md");
        using (var file = FileAccess.Open(path, FileAccess.ModeFlags.Write))
            file.StoreString(report.ToString());

        GD.Print($"Scenarios: {passed} passed, {failed} failed. Report: {path}");
        host.GetTree().Quit(failed == 0 ? 0 : 1);
    }

    private static string Arg(string name)
    {
        foreach (string arg in OS.GetCmdlineUserArgs())
            if (arg.StartsWith($"--{name}="))
                return arg[(name.Length + 3)..];
        return null;
    }
}
