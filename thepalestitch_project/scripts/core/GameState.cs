using Godot;
using ThePaleStitch.Character;

namespace ThePaleStitch.Core;

/// <summary>
/// Autoload. Holds what must survive a scene change: the character the player
/// built, the chosen language and whether the opening film was already shown.
/// </summary>
public partial class GameState : Node
{
    private const string SettingsPath = "user://settings.cfg";

    public static GameState Instance { get; private set; }

    /// <summary>The character built in the creation screen (a sensible default until then).</summary>
    public CharacterProfile Profile { get; private set; } = CharacterProfile.Default();

    public bool IntroSeen { get; private set; }

    public string Locale { get; private set; } = "en";

    public override void _EnterTree()
    {
        Instance = this;
        LoadSettings();
    }

    public void CommitProfile(CharacterProfile profile)
    {
        Profile = profile;
    }

    public void SetLocale(string locale)
    {
        Locale = locale;
        TranslationServer.SetLocale(locale);
        SaveSettings();
    }

    public void MarkIntroSeen()
    {
        if (IntroSeen) return;
        IntroSeen = true;
        SaveSettings();
    }

    private void LoadSettings()
    {
        var cfg = new ConfigFile();
        if (cfg.Load(SettingsPath) == Error.Ok)
        {
            Locale = cfg.GetValue("game", "locale", DefaultLocale()).AsString();
            IntroSeen = cfg.GetValue("game", "intro_seen", false).AsBool();
        }
        else
        {
            Locale = DefaultLocale();
        }
        TranslationServer.SetLocale(Locale);
    }

    private void SaveSettings()
    {
        var cfg = new ConfigFile();
        cfg.SetValue("game", "locale", Locale);
        cfg.SetValue("game", "intro_seen", IntroSeen);
        cfg.Save(SettingsPath);
    }

    private static string DefaultLocale() =>
        OS.GetLocaleLanguage() == "it" ? "it" : "en";
}
