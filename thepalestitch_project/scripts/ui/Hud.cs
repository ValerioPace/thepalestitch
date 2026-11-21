using System.Text.RegularExpressions;
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
///
/// The hint line has two channels. A <b>context</b> hint explains the action in
/// progress and stays until replaced (the lock). A <b>notice</b> is temporary
/// (tutorial, bell); while it shows it covers the context hint, and when it
/// expires the context hint comes back.
///
/// Hint texts may name keys as placeholders, <c>{interact}</c> or <c>{move}</c>:
/// they are filled with the keys actually bound in the Input Map.
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

    private static readonly Regex KeyToken = new(@"\{(\w+)\}");

    private PlayerController _player;
    private string _contextKey = "";
    private string _noticeKey = "";
    private float _noticeLeft;
    private Tween _hintTween;

    /// <summary>Translation key of the hint currently shown, or empty. Used by the tests.</summary>
    public string VisibleHintKey => _noticeKey != "" ? _noticeKey : _contextKey;

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
        _hint.AutoTranslateMode = AutoTranslateModeEnum.Disabled;
        _hint.Modulate = new Color(1, 1, 1, 0);
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

        if (_noticeLeft > 0f)
        {
            _noticeLeft -= (float)delta;
            if (_noticeLeft <= 0f)
            {
                _noticeKey = "";
                RefreshHint();
            }
        }
    }

    public override void _Notification(int what)
    {
        if (what == NotificationTranslationChanged && _hint != null) RefreshHint(fade: false);
    }

    public void SetObjective(string key) => _objective.Text = key;

    /// <summary>Hint for the action in progress; stays until replaced. Empty clears it.</summary>
    public void SetContextHint(string key)
    {
        _contextKey = key ?? "";
        RefreshHint();
    }

    /// <summary>Temporary hint shown for <paramref name="seconds"/>, over the context hint.</summary>
    public void Notify(string key, float seconds)
    {
        _noticeKey = key ?? "";
        _noticeLeft = seconds;
        RefreshHint();
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
        if (tolling) Notify("HINT_BELL", 6f);
    }

    public void ShowBanner(string titleKey, string bodyKey)
    {
        _contextKey = "";
        _noticeKey = "";
        RefreshHint();
        _bannerTitle.Text = titleKey;
        _bannerBody.Text = bodyKey;
        _banner.Modulate = new Color(1, 1, 1, 0);
        _banner.Visible = true;
        CreateTween().TweenProperty(_banner, "modulate:a", 1f, 1.2f);
    }

    private void RefreshHint(bool fade = true)
    {
        string key = VisibleHintKey;
        if (key != "") _hint.Text = FillKeys(Tr(key));
        float alpha = key == "" ? 0f : 1f;

        _hintTween?.Kill();
        if (!fade)
        {
            _hint.Modulate = new Color(1, 1, 1, alpha);
            return;
        }
        _hintTween = CreateTween();
        _hintTween.TweenProperty(_hint, "modulate:a", alpha, 0.4f);
    }

    /// <summary>Replace {action} placeholders with the bound keys. {move} stands for the four movement keys.</summary>
    public static string FillKeys(string text) =>
        KeyToken.Replace(text, match =>
        {
            string token = match.Groups[1].Value;
            if (token == "move")
                return KeyLabel(InputActions.MoveForward) + KeyLabel(InputActions.MoveLeft)
                     + KeyLabel(InputActions.MoveBack) + KeyLabel(InputActions.MoveRight);
            return InputMap.HasAction(token) ? KeyLabel(token) : match.Value;
        });

    /// <summary>The key bound to an action, as printed on the player's keyboard.</summary>
    public static string KeyLabel(StringName action)
    {
        foreach (InputEvent e in InputMap.ActionGetEvents(action))
        {
            if (e is not InputEventKey key) continue;
            // Without a display there is no keyboard layout to ask: use the physical key.
            bool layout = DisplayServer.GetName() != "headless";
            Key code = key.PhysicalKeycode != Key.None
                ? layout ? DisplayServer.KeyboardGetKeycodeFromPhysical(key.PhysicalKeycode) : key.PhysicalKeycode
                : key.Keycode;
            if (code == Key.None) code = key.PhysicalKeycode;
            return OS.GetKeycodeString(code);
        }
        return "?";
    }
}
