using Godot;
using ThePaleStitch.AI;
using ThePaleStitch.Core;
using ThePaleStitch.Player;
using ThePaleStitch.Stealth;
using ThePaleStitch.World;

namespace ThePaleStitch.UI;

/// <summary>
/// The play screen. Kept thin on purpose: one eye for light, one bar for noise,
/// one line for what to do. Everything else belongs to the world.
/// </summary>
public partial class Hud : CanvasLayer
{
    private Label _objective;
    private Label _hint;
    private Label _prompt;
    private Label _stance;
    private Label _surface;
    private Label _alert;
    private Label _bell;
    private ProgressBar _noise;
    private ProgressBar _progress;
    private LightGem _gem;
    private Control _firstPerson;
    private Control _banner;
    private Label _bannerTitle;
    private Label _bannerBody;

    private PlayerController _player;
    private float _hintLeft;

    public override void _Ready()
    {
        _objective = GetNode<Label>("%Objective");
        _hint = GetNode<Label>("%Hint");
        _prompt = GetNode<Label>("%Prompt");
        _stance = GetNode<Label>("%Stance");
        _surface = GetNode<Label>("%Surface");
        _alert = GetNode<Label>("%Alert");
        _bell = GetNode<Label>("%Bell");
        _noise = GetNode<ProgressBar>("%Noise");
        _progress = GetNode<ProgressBar>("%ActionProgress");
        _gem = GetNode<LightGem>("%LightGem");
        _firstPerson = GetNode<Control>("%FirstPersonOverlay");
        _banner = GetNode<Control>("%Banner");
        _bannerTitle = GetNode<Label>("%BannerTitle");
        _bannerBody = GetNode<Label>("%BannerBody");

        _hint.Text = "";
        _prompt.Text = "";
        _alert.Text = "";
        _bell.Visible = false;
        _progress.Visible = false;
        _firstPerson.Visible = false;
        _banner.Visible = false;
    }

    public override void _Process(double delta)
    {
        _player ??= GetTree().GetFirstNodeInGroup(PlayerController.Group) as PlayerController;
        if (_player == null) return;

        _gem.Value = _player.Visibility;
        _noise.Value = _player.NoiseLevel;
        _stance.Text = $"STANCE_{_player.CurrentStance.ToString().ToUpperInvariant()}";
        _surface.Text = SurfaceZone.NameKey(_player.CurrentSurface);

        _prompt.Text = _player.Focus != null && !_player.InputLocked
            ? $"[{KeyLabel(InputActions.Interact)}]  {Tr(_player.Focus.PromptKey)}"
            : "";

        float worst = 0f;
        bool alarm = false;
        foreach (Node node in GetTree().GetNodesInGroup(Watchman.Group))
        {
            if (node is not Watchman watchman) continue;
            worst = Mathf.Max(worst, watchman.Suspicion);
            alarm |= watchman.Current == Watchman.State.Alert;
        }
        _alert.Text = alarm ? "!" : worst > 0.25f ? "?" : "";
        _alert.Modulate = alarm
            ? new Color("b3261e")
            : new Color(0.85f, 0.82f, 0.75f, Mathf.Clamp(worst * 1.4f, 0.3f, 1f));

        if (_hintLeft > 0f)
        {
            _hintLeft -= (float)delta;
            if (_hintLeft <= 0f) FadeHint(0f);
        }
    }

    public void SetObjective(string key) => _objective.Text = key;

    /// <summary>Show a tutorial line. <paramref name="seconds"/> = 0 keeps it until replaced; an empty key clears it.</summary>
    public void ShowHint(string key, float seconds)
    {
        _hintLeft = seconds;
        if (string.IsNullOrEmpty(key))
        {
            FadeHint(0f);
            return;
        }
        _hint.Text = key;
        FadeHint(1f);
    }

    /// <summary>Progress of a first-person action, 0..1. Negative hides the bar.</summary>
    public void SetProgress(float value)
    {
        _progress.Visible = value >= 0f;
        _progress.Value = Mathf.Clamp(value, 0f, 1f);
    }

    public void SetFirstPerson(bool on) => _firstPerson.Visible = on;

    public void SetBell(bool tolling)
    {
        _bell.Visible = tolling;
        if (tolling) ShowHint("HINT_BELL", 6f);
    }

    public void ShowBanner(string titleKey, string bodyKey)
    {
        _bannerTitle.Text = titleKey;
        _bannerBody.Text = bodyKey;
        _banner.Modulate = new Color(1, 1, 1, 0);
        _banner.Visible = true;
        CreateTween().TweenProperty(_banner, "modulate:a", 1f, 1.2f);
    }

    private void FadeHint(float alpha) =>
        CreateTween().TweenProperty(_hint, "modulate:a", alpha, 0.4f);

    /// <summary>The key bound to an action, as printed on the player's keyboard.</summary>
    public static string KeyLabel(StringName action)
    {
        foreach (InputEvent e in InputMap.ActionGetEvents(action))
        {
            if (e is not InputEventKey key) continue;
            Key code = key.PhysicalKeycode != Key.None
                ? DisplayServer.KeyboardGetKeycodeFromPhysical(key.PhysicalKeycode)
                : key.Keycode;
            if (code == Key.None) code = key.PhysicalKeycode;
            return OS.GetKeycodeString(code);
        }
        return "?";
    }
}
