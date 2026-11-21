using System;
using System.IO;

namespace ThePaleStitch.Tests.Framework;

/// <summary>Locates files of the repository from wherever the tests are run.</summary>
public static class Repo
{
    public static string Root { get; } = FindRoot();

    public static string Project => Path.Combine(Root, "thepalestitch_project");

    public static string Read(params string[] parts) => File.ReadAllText(Path.Combine(Root, Path.Combine(parts)));

    private static string FindRoot()
    {
        string dir = AppContext.BaseDirectory;
        while (dir != null && !Directory.Exists(Path.Combine(dir, "thepalestitch_project")))
            dir = Path.GetDirectoryName(dir);
        return dir ?? throw new InvalidOperationException("Repository root not found.");
    }
}
