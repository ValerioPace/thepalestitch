using System;
using Godot;
using ThePaleStitch.Character;
using ThePaleStitch.Core;
using ThePaleStitch.Player;

namespace ThePaleStitch.UI;

/// <summary>
/// "Who holds the needle": three short steps. The man is always a thief and a
/// murderer; the player only decides what he is best at (trade), what he wears
/// (garb and its dye) and what the years have done to his face (visage).
/// </summary>
public partial class CharacterCreation : Control
{
    private enum Step
    {
        Trade,
        Garb,
        Visage,
    }

    private CharacterProfile _draft;
    private Step _step;

    private Button[] _stepButtons;
    private VBoxContainer _options;
    private Label _detailName;
    private Label _detailDesc;
    private VBoxContainer _detailExtra;
    private PortraitCard _portrait;
    private Mannequin _mannequin;
    private Label _alias;
    private Label _summary;
    private Button _back;
    private Button _next;

    public override void _Ready()
    {
        _draft = GameState.Instance.Profile.Clone();

        _stepButtons = new[]
        {
            GetNode<Button>("%StepTrade"),
            GetNode<Button>("%StepGarb"),
            GetNode<Button>("%StepVisage"),
        };
        _options = GetNode<VBoxContainer>("%Options");
        _detailName = GetNode<Label>("%DetailName");
        _detailDesc = GetNode<Label>("%DetailDesc");
        _detailExtra = GetNode<VBoxContainer>("%DetailExtra");
        _portrait = GetNode<PortraitCard>("%Portrait");
        _mannequin = GetNode<Mannequin>("%Mannequin");
        _alias = GetNode<Label>("%Alias");
        _summary = GetNode<Label>("%Summary");
        _back = GetNode<Button>("%Back");
        _next = GetNode<Button>("%Next");

        for (int i = 0; i < _stepButtons.Length; i++)
        {
            var step = (Step)i;
            _stepButtons[i].Pressed += () => ShowStep(step);
        }
        GetNode<Button>("%AliasPrev").Pressed += () => CycleAlias(-1);
        GetNode<Button>("%AliasNext").Pressed += () => CycleAlias(+1);
        _back.Pressed += GoBack;
        _next.Pressed += GoNext;

        ShowStep(Step.Trade);
    }

    public override void _Notification(int what)
    {
        // Texts composed with Tr() must be rebuilt when the language changes.
        if (what == NotificationTranslationChanged && _options != null) Refresh();
    }

    private void ShowStep(Step step)
    {
        _step = step;
        for (int i = 0; i < _stepButtons.Length; i++)
            _stepButtons[i].SetPressedNoSignal(i == (int)step);

        foreach (Node child in _options.GetChildren()) child.QueueFree();
        var group = new ButtonGroup();

        switch (step)
        {
            case Step.Trade:
                foreach (TradeDef trade in CharacterCatalog.Trades)
                    AddOption(group, trade.NameKey, trade.Id == _draft.TradeId, () => _draft.TradeId = trade.Id);
                break;
            case Step.Garb:
                foreach (CostumeDef costume in CharacterCatalog.Costumes)
                    AddOption(group, costume.NameKey, costume.Id == _draft.CostumeId, () =>
                    {
                        _draft.CostumeId = costume.Id;
                        _draft.VariantId = costume.Variants[0].Id;
                    });
                break;
            case Step.Visage:
                foreach (VisageDef visage in CharacterCatalog.Visages)
                    AddOption(group, visage.NameKey, visage.Id == _draft.VisageId, () => _draft.VisageId = visage.Id);
                break;
        }

        _back.Text = step == Step.Trade ? "CC_BACK_MENU" : "CC_BACK";
        _next.Text = step == Step.Visage ? "CC_BEGIN" : "CC_NEXT";
        Refresh();
    }

    private void AddOption(ButtonGroup group, string nameKey, bool selected, Action choose)
    {
        var button = new Button
        {
            Text = nameKey,
            ToggleMode = true,
            ButtonGroup = group,
            ButtonPressed = selected,
            Alignment = HorizontalAlignment.Left,
            CustomMinimumSize = new Vector2(0, 64),
            ThemeTypeVariation = "OptionButtonFlat",
        };
        button.Pressed += () =>
        {
            choose();
            Refresh();
        };
        _options.AddChild(button);
    }

    private void Refresh()
    {
        foreach (Node child in _detailExtra.GetChildren()) child.QueueFree();

        switch (_step)
        {
            case Step.Trade:
                TradeDef trade = _draft.Trade;
                _detailName.Text = trade.NameKey;
                _detailDesc.Text = trade.DescKey;
                AddLine($"{Tr(trade.PerkNameKey)}. {Tr(trade.PerkDescKey)}", "Perk");
                AddSpacer();
                foreach (Skill skill in CharacterCatalog.AllSkills)
                    AddSkillRow(skill, trade[skill]);
                break;

            case Step.Garb:
                CostumeDef costume = _draft.Costume;
                _detailName.Text = costume.NameKey;
                _detailDesc.Text = costume.DescKey;
                AddLine(Tr(costume.GuiseKey), "Perk");
                AddSpacer();
                AddLine($"{Tr("MOD_SEEN")}:  {Percent(costume.Visibility)}", Tone(costume.Visibility));
                AddLine($"{Tr("MOD_HEARD")}:  {Percent(costume.Noise)}", Tone(costume.Noise));
                AddLine($"{Tr("MOD_BELT")}:  {costume.BeltSlots}", "Plain");
                AddSpacer();
                AddVariantRow(costume);
                break;

            case Step.Visage:
                VisageDef visage = _draft.Visage;
                _detailName.Text = visage.NameKey;
                _detailDesc.Text = visage.DescKey;
                AddSpacer();
                AddLine(Tr("CC_VISAGE_RULE"), "Faint");
                break;
        }

        _portrait.Portray(_draft.Visage, _draft.Variant);
        _mannequin.Apply(_draft.Variant);
        _alias.Text = CharacterCatalog.AliasKey(_draft.Alias);
        _summary.Text =
            $"{Tr(CharacterCatalog.AliasKey(_draft.Alias))}  ·  {Tr(_draft.Trade.NameKey)}  ·  " +
            $"{Tr(_draft.Costume.NameKey)} ({Tr(_draft.Variant.NameKey)})  ·  {Tr(_draft.Visage.NameKey)}";
    }

    private void AddSkillRow(Skill skill, int value)
    {
        var row = new HBoxContainer { TooltipText = Tr(CharacterCatalog.SkillHintKey(skill)) };
        row.AddThemeConstantOverride("separation", 24);

        var name = new Label
        {
            Text = CharacterCatalog.SkillNameKey(skill),
            CustomMinimumSize = new Vector2(190, 0),
            MouseFilter = MouseFilterEnum.Ignore,
        };
        var pips = new PipBar { Value = value, Max = CharacterCatalog.MaxSkill, MouseFilter = MouseFilterEnum.Ignore };
        var hint = new Label
        {
            Text = CharacterCatalog.SkillHintKey(skill),
            ThemeTypeVariation = "FaintLabel",
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            AutowrapMode = TextServer.AutowrapMode.WordSmart,
            MouseFilter = MouseFilterEnum.Ignore,
        };

        row.AddChild(name);
        row.AddChild(pips);
        row.AddChild(hint);
        _detailExtra.AddChild(row);
    }

    private void AddVariantRow(CostumeDef costume)
    {
        var row = new HBoxContainer();
        row.AddThemeConstantOverride("separation", 16);
        var group = new ButtonGroup();

        foreach (VariantDef variant in costume.Variants)
        {
            var button = new Button
            {
                Text = variant.NameKey,
                Icon = Swatch(variant.Cloth, variant.Trim),
                ToggleMode = true,
                ButtonGroup = group,
                ButtonPressed = variant.Id == _draft.VariantId,
                CustomMinimumSize = new Vector2(0, 56),
            };
            button.Pressed += () =>
            {
                _draft.VariantId = variant.Id;
                Refresh();
            };
            row.AddChild(button);
        }
        _detailExtra.AddChild(row);
    }

    private void AddLine(string text, string tone)
    {
        var label = new Label
        {
            Text = text,
            AutowrapMode = TextServer.AutowrapMode.WordSmart,
            // Already translated: do not look the sentence up as a key again.
            AutoTranslateMode = AutoTranslateModeEnum.Disabled,
        };
        switch (tone)
        {
            case "Perk": label.ThemeTypeVariation = "PerkLabel"; break;
            case "Faint": label.ThemeTypeVariation = "FaintLabel"; break;
            case "Good": label.AddThemeColorOverride("font_color", new Color("9aa886")); break;
            case "Bad": label.AddThemeColorOverride("font_color", new Color("b0584c")); break;
        }
        _detailExtra.AddChild(label);
    }

    private void AddSpacer() =>
        _detailExtra.AddChild(new Control { CustomMinimumSize = new Vector2(0, 14) });

    private void CycleAlias(int by)
    {
        string[] all = CharacterCatalog.Aliases;
        int index = Array.IndexOf(all, _draft.Alias);
        _draft.Alias = all[((index < 0 ? 0 : index) + by + all.Length) % all.Length];
        Refresh();
    }

    private void GoBack()
    {
        if (_step == Step.Trade) SceneRouter.Instance.GoTo(SceneRouter.MainMenu);
        else ShowStep(_step - 1);
    }

    private void GoNext()
    {
        if (_step != Step.Visage)
        {
            ShowStep(_step + 1);
            return;
        }
        GameState.Instance.CommitProfile(_draft);
        SceneRouter.Instance.GoTo(SceneRouter.Prologue, 1.2f);
    }

    /// <summary>Multiplier as a signed percentage: 0.8 reads "-20%".</summary>
    private static string Percent(float multiplier)
    {
        int percent = Mathf.RoundToInt((multiplier - 1f) * 100f);
        return percent == 0 ? "0%" : $"{percent:+0;-0}%";
    }

    private static string Tone(float multiplier) =>
        multiplier < 0.999f ? "Good" : multiplier > 1.001f ? "Bad" : "Plain";

    private static ImageTexture Swatch(Color cloth, Color trim)
    {
        const int size = 28;
        Image image = Image.CreateEmpty(size, size, false, Image.Format.Rgba8);
        image.Fill(cloth);
        image.FillRect(new Rect2I(0, size - 7, size, 7), trim);
        return ImageTexture.CreateFromImage(image);
    }
}
