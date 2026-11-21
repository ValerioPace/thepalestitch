using System.Reflection;
using ThePaleStitch.Tests.Framework;

namespace ThePaleStitch.Tests;

public static class Program
{
    /// <summary>Usage: dotnet run --project tests/ThePaleStitch.Tests [-- filter]</summary>
    public static int Main(string[] args) =>
        Runner.RunAll(Assembly.GetExecutingAssembly(), args.Length > 0 ? args[0] : null) == 0 ? 0 : 1;
}
