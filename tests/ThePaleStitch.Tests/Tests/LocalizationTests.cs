using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ThePaleStitch.Tests.Framework;
using Xunit;

namespace ThePaleStitch.Tests;

/// <summary>strings.csv is complete, consistent, and covers the keys used in code and scenes.</summary>
public class LocalizationTests
{
    private static readonly List<string[]> Rows =
        GodotFiles.ReadCsv(Repo.Read("thepalestitch_project", "localization", "strings.csv"));

    private static readonly HashSet<string> Keys = Rows.Skip(1).Select(r => r[0]).ToHashSet();

    [Fact]
    public void Header_is_keys_en_it() => Assert.Equal("keys,en,it", string.Join(",", Rows[0]));

    [Fact]
    public void Every_row_has_both_languages()
    {
        var bad = Rows.Skip(1).Where(r => r.Length != 3 || r.Any(string.IsNullOrWhiteSpace)).Select(r => r[0]);
        Assert.Empty(bad);
    }

    [Fact]
    public void Keys_are_unique()
    {
        var duplicates = Rows.Skip(1).GroupBy(r => r[0]).Where(g => g.Count() > 1).Select(g => g.Key);
        Assert.Empty(duplicates);
    }

    [Fact]
    public void Key_placeholders_match_between_languages_and_name_real_actions()   // FIX-009
    {
        var input = GodotFiles.Section(Repo.Read("thepalestitch_project", "project.godot"), "input");
        var problems = new List<string>();
        foreach (string[] row in Rows.Skip(1))
        {
            var en = Placeholders(row[1]);
            var it = Placeholders(row[2]);
            if (!en.SetEquals(it)) problems.Add($"{row[0]}: en {{{string.Join(",", en)}}} it {{{string.Join(",", it)}}}");
            problems.AddRange(en.Where(p => p != "move" && !input.ContainsKey(p)).Select(p => $"{row[0]}: unknown action {p}"));
        }
        Assert.Empty(problems);
    }

    [Fact]
    public void No_hint_names_a_key_literally()   // FIX-009
    {
        var literal = new Regex(@"\b(WASD|Ctrl|Shift)\b|\b[EQRSV]\b(?= (to|per|e|and)\b)");
        var bad = Rows.Skip(1).Where(r => r[0].StartsWith("HINT_") && (literal.IsMatch(r[1]) || literal.IsMatch(r[2]))).Select(r => r[0]);
        Assert.Empty(bad);
    }

    [Fact]
    public void Keys_written_in_code_and_scenes_exist()
    {
        var prefixes = "GAME|BOOT|MENU|INTRO|CC|MOD|SKILL|STANCE|SURFACE|HUD|PROMPT|PROLOGUE|HINT|CAUGHT|PAUSE";
        var literal = new Regex($"\"((?:{prefixes})_[A-Z0-9_]+)\"");
        var missing = new SortedSet<string>();
        foreach (string file in Directory.EnumerateFiles(Repo.Project, "*.*", SearchOption.AllDirectories)
                     .Where(f => (f.EndsWith(".cs") || f.EndsWith(".tscn")) && !f.Contains(Path.DirectorySeparatorChar + ".godot")))
            foreach (Match m in literal.Matches(File.ReadAllText(file)))
                if (!Keys.Contains(m.Groups[1].Value)) missing.Add($"{Path.GetFileName(file)}: {m.Groups[1].Value}");
        Assert.Empty(missing);
    }

    private static HashSet<string> Placeholders(string text) =>
        Regex.Matches(text, @"\{(\w+)\}").Select(m => m.Groups[1].Value).ToHashSet();
}
