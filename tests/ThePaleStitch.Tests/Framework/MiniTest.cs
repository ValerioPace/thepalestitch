using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// A deliberately tiny, dependency-free subset of the xUnit API: [Fact],
// [Theory] + [InlineData] and the Assert calls the suite uses. Same names and
// semantics as xUnit, so switching to the real package means deleting this
// folder and adding three PackageReferences.
namespace Xunit
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FactAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class TheoryAttribute : FactAttribute { }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class InlineDataAttribute : Attribute
    {
        public object[] Data { get; }
        public InlineDataAttribute(params object[] data) => Data = data;
    }

    public sealed class AssertException : Exception
    {
        public AssertException(string message) : base(message) { }
    }

    public static class Assert
    {
        public static void True(bool condition, string message = null)
        {
            if (!condition) throw new AssertException(message ?? "Expected true.");
        }

        public static void False(bool condition, string message = null)
        {
            if (condition) throw new AssertException(message ?? "Expected false.");
        }

        public static void Equal<T>(T expected, T actual)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
                throw new AssertException($"Expected: {expected}\nActual:   {actual}");
        }

        /// <summary>Floating point equality to <paramref name="precision"/> decimal places, like xUnit.</summary>
        public static void Equal(double expected, double actual, int precision)
        {
            if (Math.Round(expected - actual, precision) != 0)
                throw new AssertException($"Expected: {expected}\nActual:   {actual} (precision {precision})");
        }

        public static void InRange<T>(T actual, T low, T high) where T : IComparable<T>
        {
            if (actual.CompareTo(low) < 0 || actual.CompareTo(high) > 0)
                throw new AssertException($"{actual} is not in [{low}, {high}]");
        }

        public static void Empty<T>(IEnumerable<T> collection)
        {
            var items = collection.ToList();
            if (items.Count > 0)
                throw new AssertException("Expected empty, found:\n  " + string.Join("\n  ", items));
        }

        public static void Contains(string expected, string actual)
        {
            if (actual == null || !actual.Contains(expected))
                throw new AssertException($"'{expected}' not found in '{actual}'");
        }
    }
}

namespace ThePaleStitch.Tests.Framework
{
    using Xunit;

    public static class Runner
    {
        /// <summary>Runs every [Fact] and [Theory] case in the assembly. Returns the number of failures.</summary>
        public static int RunAll(Assembly assembly, string filter)
        {
            int passed = 0, failed = 0;
            var failures = new List<string>();

            var tests = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance), (t, m) => (t, m))
                .Where(x => x.m.GetCustomAttribute<FactAttribute>() != null)
                .Where(x => filter == null || $"{x.t.Name}.{x.m.Name}".Contains(filter, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.t.Name).ThenBy(x => x.m.Name);

            foreach (var (type, method) in tests)
            {
                var cases = method.GetCustomAttribute<TheoryAttribute>() != null
                    ? method.GetCustomAttributes<InlineDataAttribute>().Select(d => d.Data).ToList()
                    : new List<object[]> { Array.Empty<object>() };

                foreach (object[] args in cases)
                {
                    string name = $"{type.Name}.{method.Name}" + (args.Length > 0 ? $"({string.Join(", ", args)})" : "");
                    try
                    {
                        object instance = Activator.CreateInstance(type);
                        method.Invoke(instance, Convert(method, args));
                        passed++;
                        Console.WriteLine($"  PASS  {name}");
                    }
                    catch (TargetInvocationException e)
                    {
                        failed++;
                        string message = e.InnerException?.Message ?? e.Message;
                        failures.Add($"{name}\n      {message.Replace("\n", "\n      ")}");
                        Console.WriteLine($"  FAIL  {name}");
                    }
                }
            }

            Console.WriteLine();
            foreach (string f in failures) Console.WriteLine($"FAILED {f}\n");
            Console.WriteLine($"Unit tests: {passed} passed, {failed} failed.");
            return failed;
        }

        private static object[] Convert(MethodInfo method, object[] args)
        {
            ParameterInfo[] parameters = method.GetParameters();
            var converted = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
                converted[i] = args[i] == null ? null : System.Convert.ChangeType(args[i], parameters[i].ParameterType);
            return converted;
        }
    }
}
