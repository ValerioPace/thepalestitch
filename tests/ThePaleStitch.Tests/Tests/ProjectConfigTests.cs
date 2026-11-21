using System.Linq;
using System.Text.RegularExpressions;
using ThePaleStitch.Core;
using ThePaleStitch.Tests.Framework;
using Xunit;

namespace ThePaleStitch.Tests;

/// <summary>project.godot and the scenes agree with the code (CR-008).</summary>
public class ProjectConfigTests
{
    private static readonly string Project = Repo.Read("thepalestitch_project", "project.godot");

    [Fact]
    public void Layer_names_match_PhysicsLayers()
    {
        var names = GodotFiles.Section(Project, "layer_names");
        for (int i = 0; i < PhysicsLayers.Names.Length; i++)
            Assert.Equal($"\"{PhysicsLayers.Names[i]}\"", names[$"3d_physics/layer_{i + 1}"]);
    }

    [Fact]
    public void Every_InputActions_name_is_in_the_input_map()
    {
        var input = GodotFiles.Section(Project, "input");
        string code = Repo.Read("thepalestitch_project", "scripts", "core", "InputActions.cs");
        var missing = Regex.Matches(code, "StringName \\w+ = \"(\\w+)\"")
            .Select(m => m.Groups[1].Value)
            .Where(action => !input.ContainsKey(action));
        Assert.Empty(missing);
    }

    [Fact]
    public void Sneak_shows_C_first()   // CR-022: the tutorial names the first key bound
    {
        string sneak = GodotFiles.Section(Project, "input")["sneak"];
        Match first = Regex.Match(sneak, "\"physical_keycode\":(\\d+)");
        Assert.Equal("67", first.Groups[1].Value);
    }

    [Fact]
    public void Physics_interpolation_is_on()   // FIX-002
    {
        var physics = GodotFiles.Section(Project, "physics");
        Assert.Equal("true", physics["common/physics_interpolation"]);
        Assert.Equal("0.0", physics["common/physics_jitter_fix"]);
    }

    [Fact]
    public void Player_and_guard_collide()   // FIX-001
    {
        var player = GodotFiles.Node(Repo.Read("thepalestitch_project", "scenes", "player", "Player.tscn"), "Player");
        var guard = GodotFiles.Node(Repo.Read("thepalestitch_project", "scenes", "ai", "Watchman.tscn"), "Watchman");
        Assert.Equal(PhysicsLayers.Player, GodotFiles.UInt(player, "collision_layer", 1));
        Assert.Equal(PhysicsLayers.PlayerMask, GodotFiles.UInt(player, "collision_mask", 1));
        Assert.Equal(PhysicsLayers.Guards, GodotFiles.UInt(guard, "collision_layer", 1));
        Assert.Equal(PhysicsLayers.GuardMask, GodotFiles.UInt(guard, "collision_mask", 1));
    }

    [Fact]
    public void Prologue_zones_and_door_use_the_right_layers()
    {
        string level = Repo.Read("thepalestitch_project", "thepalestitch_prologue.tscn");
        foreach (string zone in new[] { "Straw", "GravelStreet", "GravelYard", "YardPlanks", "AlleyMud" })
        {
            var props = GodotFiles.Node(level, zone, "Surfaces");
            Assert.Equal(PhysicsLayers.Surfaces, GodotFiles.UInt(props, "collision_layer", 1));
            Assert.Equal(PhysicsLayers.Player, GodotFiles.UInt(props, "collision_mask", 1));
        }
        var door = GodotFiles.Node(level, "MerchantDoor", ".");
        Assert.Equal(PhysicsLayers.Interactables, GodotFiles.UInt(door, "collision_layer", 1));
    }
}
