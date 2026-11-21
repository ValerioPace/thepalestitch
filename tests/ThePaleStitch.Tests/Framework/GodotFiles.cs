using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ThePaleStitch.Tests.Framework;

/// <summary>Small readers for the Godot text formats the tests inspect.</summary>
public static class GodotFiles
{
    /// <summary>Rows of a CSV with quoted fields ("" escapes a quote).</summary>
    public static List<string[]> ReadCsv(string text)
    {
        var rows = new List<string[]>();
        var fields = new List<string>();
        var field = new StringBuilder();
        bool quoted = false;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (quoted)
            {
                if (c == '"' && i + 1 < text.Length && text[i + 1] == '"') { field.Append('"'); i++; }
                else if (c == '"') quoted = false;
                else field.Append(c);
            }
            else if (c == '"') quoted = true;
            else if (c == ',') { fields.Add(field.ToString()); field.Clear(); }
            else if (c == '\n' || c == '\r')
            {
                if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;
                fields.Add(field.ToString()); field.Clear();
                if (fields.Count > 1 || fields[0].Length > 0) rows.Add(fields.ToArray());
                fields.Clear();
            }
            else field.Append(c);
        }
        if (field.Length > 0 || fields.Count > 0) { fields.Add(field.ToString()); rows.Add(fields.ToArray()); }
        return rows;
    }

    /// <summary>Lines "key=value" of one [section] of an INI-like Godot file.</summary>
    public static Dictionary<string, string> Section(string text, string section)
    {
        var result = new Dictionary<string, string>();
        bool inside = false;
        string pendingKey = null;
        var pending = new StringBuilder();
        foreach (string raw in text.Split('\n'))
        {
            string line = raw.TrimEnd('\r');
            if (pendingKey != null)
            {
                pending.Append('\n').Append(line);
                if (line == "}") { result[pendingKey] = pending.ToString(); pendingKey = null; }
                continue;
            }
            if (line.StartsWith("[")) { inside = line == $"[{section}]"; continue; }
            if (!inside || line.Length == 0 || line.StartsWith(";")) continue;
            int eq = line.IndexOf('=');
            if (eq < 0) continue;
            string key = line[..eq], value = line[(eq + 1)..];
            if (value == "{") { pendingKey = key; pending.Clear().Append(value); continue; }
            result[key] = value;
        }
        return result;
    }

    /// <summary>The property lines of the node called <paramref name="name"/> in a .tscn.</summary>
    public static Dictionary<string, string> Node(string tscn, string name, string parent = null)
    {
        string header = $"[node name=\"{name}\"";
        string[] blocks = Regex.Split(tscn, @"\n(?=\[)");
        foreach (string block in blocks)
        {
            string first = block.Split('\n')[0];
            if (!first.StartsWith(header)) continue;
            if (parent != null && !first.Contains($"parent=\"{parent}\"")) continue;
            if (parent == null && first.Contains("parent=")) continue;
            return block.Split('\n').Skip(1)
                .Select(l => l.Split(" = ", 2))
                .Where(p => p.Length == 2)
                .ToDictionary(p => p[0], p => p[1]);
        }
        throw new InvalidOperationException($"Node {name} not found.");
    }

    public static uint UInt(Dictionary<string, string> props, string key, uint fallback) =>
        props.TryGetValue(key, out string v) ? uint.Parse(v) : fallback;
}
