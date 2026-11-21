using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using ThePaleStitch.Core;
using ThePaleStitch.Player;
using ThePaleStitch.UI;
using ThePaleStitch.World;

namespace ThePaleStitch.Dev;

/// <summary>
/// Gameplay scenarios run headless by <see cref="ScenarioRunner"/>. Each one
/// sets up a situation in a real scene, lets simulated time pass and checks
/// what the game did. Names refer to backlog items where they guard a fix.
/// </summary>
public static class Scenarios
{
    public sealed record Entry(string Name, string Covers, Func<ScenarioContext, Task> Run);

    public static readonly IReadOnlyList<Entry> All = new Entry[]
    {
        new("scenes_load", "smoke", ScenesLoad),
        new("prologue_reachability", "CR-021", PrologueReachability),
        new("player_cannot_cross_guard", "FIX-001", PlayerCannotCrossGuard),
        new("guard_hears_walking_from_behind", "regression", GuardHearsWalkingFromBehind),
        new("lock_hint_survives_bell", "FIX-004", LockHintSurvivesBell),
        new("hint_keys_are_bound_keys", "FIX-009", HintKeysAreBoundKeys),
        new("sneak_is_a_toggle", "CR-022, FIX-005", SneakIsAToggle),
        new("caught_then_quit_stays_in_menu", "FIX-014", CaughtThenQuitStaysInMenu),
        new("debug_overlay_toggles", "CR-020", DebugOverlayToggles),
        new("scene_requests_are_not_lost", "FIX-008", SceneRequestsAreNotLost),
        new("motion_is_smooth_at_144hz", "FIX-002", MotionIsSmooth),
    };

    // Prologue geometry (see thepalestitch_prologue.tscn).
    private static readonly Vector3 Start = new(-18.5f, 0f, 1.2f);
    private static readonly Rect2 PrologueBounds = new(-32f, -25f, 72f, 46f);
    private const float ExpectedArea = 382.5f;

    private static async Task ScenesLoad(ScenarioContext c)
    {
        foreach (string path in new[]
                 {
                     "res://scenes/boot/Boot.tscn", "res://scenes/intro/IntroCinematic.tscn",
                     "res://scenes/menu/MainMenu.tscn", "res://scenes/character/CharacterCreation.tscn",
                     SceneRouter.Prologue,
                 })
        {
            await c.Load(path);
            await c.Wait(1.0);
            c.Check($"loads {path.GetFile()}", c.Scene != null && c.Scene.SceneFilePath == path, c.Scene?.SceneFilePath ?? "no scene");
        }
    }

    private static async Task PrologueReachability(ScenarioContext c)
    {
        await c.Load(SceneRouter.Prologue);
        var map = new ReachabilityMap();
        await map.Build(c.Scene, c.Scene.GetNode<Node3D>("World"), Start, PrologueBounds);

        c.Check("walkable area as designed", Mathf.Abs(map.AreaSquareMetres - ExpectedArea) <= 15f,
            $"{map.AreaSquareMetres:0.#} m² (expected {ExpectedArea} ± 15)");
        c.Check("alley reachable", map.CanReach(Start, new Vector3(-8.5f, 0, -9.5f)));
        c.Check("yard reachable through the gate", map.CanReach(Start, new Vector3(22f, 0, -6f)));
        foreach (var (name, point) in new[]
                 {
                     ("behind the houses", new Vector3(0, 0, -14)),
                     ("west end, behind", new Vector3(-20.2f, 0, -6)),
                     ("gap between houses", new Vector3(4, 0, -6)),
                     ("west side of the yard", new Vector3(16, 0, -10)),
                     ("east side of the yard", new Vector3(28.2f, 0, -10)),
                     ("behind the merchant's house", new Vector3(22, 0, -20)),
                     ("south of the parapet", new Vector3(0, 0, 8)),
                 })
            c.Check($"no way {name}", !map.CanReach(Start, point));

        string png = c.ReportDir.PathJoin("prologue_reachability.png");
        map.Picture.SavePng(png);
        c.Note($"Map: {png}");
        map.Free();
    }

    private static async Task PlayerCannotCrossGuard(ScenarioContext c)
    {
        await c.Load(SceneRouter.Prologue);
        await c.Wait(0.5);
        var player = c.Player;
        var guard = c.Guard;

        // Put the player two metres in front of the guard and walk into him.
        Vector3 forward = -guard.GlobalBasis.Z;
        player.GlobalPosition = guard.GlobalPosition + forward * 2f + Vector3.Up * 0.05f;
        await c.Frames(2);
        PressTowards(-forward);

        float closest = float.MaxValue;
        for (int i = 0; i < 90; i++)
        {
            await c.Frames(1);
            closest = Mathf.Min(closest, ScenarioContext.Flat(player.GlobalPosition, guard.GlobalPosition));
        }
        ReleaseMovement();
        c.Check("bodies never overlap", closest >= 0.6f, $"closest {closest:0.00} m (capsules 0.35 + 0.35)");
    }

    private static async Task GuardHearsWalkingFromBehind(ScenarioContext c)
    {
        await c.Load(SceneRouter.Prologue);
        await c.Wait(0.5);
        var player = c.Player;
        var guard = c.Guard;
        Vector3 forward = -guard.GlobalBasis.Z;
        player.GlobalPosition = guard.GlobalPosition - forward * 7f + Vector3.Up * 0.05f;
        await c.Frames(2);
        PressTowards(forward);

        double seconds = 0;
        while (seconds < 8 && guard.Current != AI.Watchman.State.Alert)
        {
            await c.Wait(0.1);
            seconds += 0.1;
            if (ScenarioContext.Flat(player.GlobalPosition, guard.GlobalPosition) < 2f) ReleaseMovement();
        }
        ReleaseMovement();
        c.Check("walking up behind a guard gets you caught", guard.Current == AI.Watchman.State.Alert,
            $"state {guard.Current} after {seconds:0.0} s");
    }

    private static async Task LockHintSurvivesBell(ScenarioContext c)
    {
        await c.Load(SceneRouter.Prologue);
        await c.Wait(0.3);
        var player = c.Player;
        player.GlobalPosition = new Vector3(22f, 0.05f, -6.6f);
        await c.Wait(0.3);
        var door = c.Scene.GetNode<Interactable>("MerchantDoor");
        door.Interact(player);
        await c.Wait(0.5);
        Hud hud = c.Hud;
        c.Check("lock hint shown", hud.VisibleHintKey == "HINT_LOCKPICK", hud.VisibleHintKey);

        hud.SetBell(true);
        await c.Wait(0.5);
        c.Check("bell notice covers it", hud.VisibleHintKey == "HINT_BELL", hud.VisibleHintKey);

        await c.Wait(6.5);
        c.Check("lock hint back after the bell", hud.VisibleHintKey == "HINT_LOCKPICK", hud.VisibleHintKey);
    }

    private static async Task HintKeysAreBoundKeys(ScenarioContext c)
    {
        await c.Load(SceneRouter.Prologue);
        string lockHint = Hud.FillKeys(TranslationServer.Translate("HINT_LOCKPICK"));
        string moveHint = Hud.FillKeys(TranslationServer.Translate("HINT_MOVE"));
        c.Check("no placeholder left", !lockHint.Contains('{') && !moveHint.Contains('{'), lockHint);
        c.Check("lock hint names E and S", lockHint.Contains(Hud.KeyLabel(InputActions.Interact)) && lockHint.Contains(Hud.KeyLabel(InputActions.MoveBack)), lockHint);
        c.Check("move hint names C for creeping", Hud.KeyLabel(InputActions.Sneak) == "C" && moveHint.Contains(" C "), moveHint);
    }

    private static async Task SneakIsAToggle(ScenarioContext c)
    {
        await c.Load(SceneRouter.Prologue);
        await c.Wait(0.3);
        var player = c.Player;

        c.Check("starts walking", player.CurrentStance == PlayerController.Stance.Walk, player.CurrentStance.ToString());
        await c.Tap(InputActions.Sneak);
        await c.Wait(0.5);
        c.Check("one press: creeping, and it stays", player.SneakToggled && player.CurrentStance == PlayerController.Stance.Sneak, player.CurrentStance.ToString());

        Input.ActionPress(InputActions.Run);
        await c.Frames(3);
        c.Check("holding run: running", player.CurrentStance == PlayerController.Stance.Run, player.CurrentStance.ToString());
        Input.ActionRelease(InputActions.Run);
        await c.Frames(3);
        c.Check("release run: back to creeping (D-04)", player.CurrentStance == PlayerController.Stance.Sneak, player.CurrentStance.ToString());

        player.GlobalPosition = new Vector3(22f, 0.05f, -6.6f);
        await c.Wait(0.3);
        c.Scene.GetNode<Interactable>("MerchantDoor").Interact(player);
        await c.Frames(3);
        c.Check("at the lock: busy, neutral visibility (D-03)",
            player.CurrentStance == PlayerController.Stance.Busy && Mathf.IsEqualApprox(player.BusyVisibility, 1f),
            $"{player.CurrentStance}, factor {player.BusyVisibility}");
        await c.Tap(InputActions.MoveBack);
        await c.Frames(3);
        c.Check("leaving the lock restores creeping", player.CurrentStance == PlayerController.Stance.Sneak, player.CurrentStance.ToString());

        await c.Tap(InputActions.Sneak);
        await c.Frames(3);
        c.Check("second press: walking", !player.SneakToggled && player.CurrentStance == PlayerController.Stance.Walk, player.CurrentStance.ToString());
    }

    private static async Task CaughtThenQuitStaysInMenu(ScenarioContext c)
    {
        await c.Load(SceneRouter.Prologue);
        await c.Wait(0.3);
        // Get caught (as the level does when a guard raises the alarm)...
        var level = c.Scene;
        typeof(PrologueLevel).GetMethod("OnAlarm", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(level, new object[] { c.Guard });
        await c.Wait(0.5);
        // ...then leave to the menu before the level restarts by itself.
        SceneRouter.Instance.GoTo(SceneRouter.MainMenu);
        await c.Wait(6.0);
        c.Check("still in the menu after the restart delay", c.Scene?.SceneFilePath == SceneRouter.MainMenu, c.Scene?.SceneFilePath);
    }

    private static async Task DebugOverlayToggles(ScenarioContext c)
    {
        await c.Load(SceneRouter.Prologue);
        await c.Wait(0.3);
        var overlay = c.Tree.Root.GetNodeOrNull<DebugOverlay>("DebugOverlay");
        c.Check("overlay exists in debug builds", overlay != null);
        if (overlay == null) return;
        await c.Tap(InputActions.DebugOverlay);
        await c.Wait(0.5);
        c.Check("F3 turns it on and the sensor records rays", overlay.On && c.Player.Sensor.RecordRays && c.Player.Sensor.Rays.Count > 0,
            $"{c.Player.Sensor.Rays.Count} rays");
        await c.Tap(InputActions.DebugOverlay);
        await c.Frames(2);
        c.Check("F3 again turns it off", !overlay.On && !c.Player.Sensor.RecordRays);
    }

    private static async Task SceneRequestsAreNotLost(ScenarioContext c)
    {
        await c.Load(SceneRouter.Prologue);
        await c.Wait(0.3);
        SceneRouter.Instance.GoTo(SceneRouter.MainMenu);
        await c.Wait(0.2);
        SceneRouter.Instance.GoTo(SceneRouter.CharacterCreation);   // arrives mid-transition
        await c.Wait(4.0);
        c.Check("the last request wins", c.Scene?.SceneFilePath == SceneRouter.CharacterCreation, c.Scene?.SceneFilePath);
        c.Check("router free again", !SceneRouter.Instance.Busy);
    }

    /// <summary>
    /// With the bench at --fixed-fps 144 the screen draws 144 frames for 60
    /// physics steps, like a fast monitor. Without interpolation the body moves
    /// on some frames and not on others; with it, every frame moves about the same.
    /// </summary>
    private static async Task MotionIsSmooth(ScenarioContext c)
    {
        await c.Load(SceneRouter.Prologue);
        await c.Wait(0.3);
        var player = c.Player;
        player.GlobalPosition = new Vector3(-17f, 0.05f, 0.5f);
        await c.Frames(2);
        player.ResetPhysicsInterpolation();
        PressTowards(Vector3.Right);
        await c.Wait(0.6);   // up to speed

        async Task<float> Unevenness()
        {
            var steps = new System.Collections.Generic.List<float>();
            Vector3 last = player.GetGlobalTransformInterpolated().Origin;
            for (int i = 0; i < 72; i++)
            {
                await c.Tree.ToSignal(c.Tree, SceneTree.SignalName.ProcessFrame);
                Vector3 now = player.GetGlobalTransformInterpolated().Origin;
                steps.Add(ScenarioContext.Flat(now, last));
                last = now;
            }
            float mean = 0f;
            foreach (float d in steps) mean += d;
            mean /= steps.Count;
            float variance = 0f;
            foreach (float d in steps) variance += (d - mean) * (d - mean);
            return Mathf.Sqrt(variance / steps.Count) / Mathf.Max(mean, 1e-5f);   // coefficient of variation
        }

        float on = await Unevenness();
        c.Tree.PhysicsInterpolation = false;
        float off = await Unevenness();
        c.Tree.PhysicsInterpolation = true;
        ReleaseMovement();

        c.Note($"frame-to-frame unevenness: {on:0.00} interpolated, {off:0.00} without (render {Engine.GetFramesPerSecond()} fps)");
        c.Check("even movement on screen", on < 0.2f, $"variation {on:0.00} (without interpolation {off:0.00})");
    }

    /// <summary>Hold the movement keys that move the player along <paramref name="world"/>, given the camera yaw.</summary>
    private static void PressTowards(Vector3 world)
    {
        float yaw = CameraRig.Current?.Yaw ?? Mathf.DegToRad(45f);
        Vector3 local = world.Rotated(Vector3.Up, -yaw);   // camera space: x right, z back
        ReleaseMovement();
        if (local.X > 0.3f) Input.ActionPress(InputActions.MoveRight, Mathf.Clamp(local.X, 0, 1));
        if (local.X < -0.3f) Input.ActionPress(InputActions.MoveLeft, Mathf.Clamp(-local.X, 0, 1));
        if (local.Z > 0.3f) Input.ActionPress(InputActions.MoveBack, Mathf.Clamp(local.Z, 0, 1));
        if (local.Z < -0.3f) Input.ActionPress(InputActions.MoveForward, Mathf.Clamp(-local.Z, 0, 1));
    }

    private static void ReleaseMovement()
    {
        Input.ActionRelease(InputActions.MoveLeft);
        Input.ActionRelease(InputActions.MoveRight);
        Input.ActionRelease(InputActions.MoveForward);
        Input.ActionRelease(InputActions.MoveBack);
    }
}
