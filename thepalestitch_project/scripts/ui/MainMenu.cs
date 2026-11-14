using Godot;
using ThePaleStitch.Core;

namespace ThePaleStitch.UI;

/// <summary>Title screen.</summary>
public partial class MainMenu : Control
{
    public override void _Ready()
    {
        var newGame = GetNode<Button>("%NewGame");
        newGame.Pressed += () => SceneRouter.Instance.GoTo(SceneRouter.CharacterCreation);
        GetNode<Button>("%WatchIntro").Pressed += () => SceneRouter.Instance.GoTo(SceneRouter.Intro);
        GetNode<Button>("%Language").Pressed += ToggleLanguage;
        GetNode<Button>("%Quit").Pressed += () => GetTree().Quit();

        // No save system yet: "Continue" is shown, dead, so the layout is final.
        GetNode<Button>("%Continue").Disabled = true;

        GetNode<Label>("%Version").Text =
            "v" + ProjectSettings.GetSetting("application/config/version", "0.0.1").AsString();

        newGame.GrabFocus();
    }

    private static void ToggleLanguage() =>
        GameState.Instance.SetLocale(GameState.Instance.Locale == "it" ? "en" : "it");
}
