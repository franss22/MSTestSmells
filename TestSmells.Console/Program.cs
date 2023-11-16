using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using System.Collections.Immutable;
using CommandLine;
using System.IO;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Options;
using System.Text.Json;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestSmells.Console
{

    internal static partial class Program
    {

        public readonly struct ProjectSummary
        {
            public int FoundSmells { get; }
            public int HiddenSmells { get; }
            public int TestClasses { get; }
            public int TestMethods { get; }
            public string Message { get; }

            public ProjectSummary(int foundSmells, int hiddenSmells, int testClasses, int testMethods, string message)
            {
                FoundSmells = foundSmells;
                HiddenSmells = hiddenSmells;
                TestClasses = testClasses;
                TestMethods = testMethods;
                Message = message;
            }

            public override readonly string ToString() { return Message; }
        }

        public static async Task Main(string[] args)
        {
            var result = await Parser.Default.ParseArguments<ConsoleProgramOptions>(args)
                .WithParsedAsync(RunProgram);
        }



        private static async Task RunProgram(ConsoleProgramOptions options)
        {
            var diagnostics = new List<Diagnostic>();

            var projectSummaries = new ConcurrentBag<ProjectSummary>();

            var analyzers = TestSmellAnalyzers();
            var analyzerIds = SupportedDagnosticsIds(analyzers);
            Dictionary<string, string?> severityConfig;

            System.Console.WriteLine("Loading analyzers");

            MSBuildLocator.RegisterDefaults();
            using (var workspace = MSBuildWorkspace.Create())
            {
                System.Console.WriteLine("Loading solution");
                var solution = await TryOpenSolutionAsync(workspace, options.Solution);
                severityConfig = SetConfig(options, analyzerIds);

                System.Console.WriteLine("Compiling Solution");
                var compilations = await Task.WhenAll(solution.Projects.Select(p => p.GetCompilationAsync()));

                System.Console.WriteLine("Analyzing Solution");
                diagnostics = await AnalyzeSolution(diagnostics, projectSummaries, analyzers, severityConfig, compilations);

                System.Console.WriteLine("Finalized Analysis");

            }

            PrintProjectSummaries(projectSummaries);

            System.Console.WriteLine($"Printing diagnostic details to {options.Output ?? "console"}");
            SaveOrPrintDiagnosticsCSV(options, diagnostics, severityConfig);

            SaveMethodSummaryCSV(options, diagnostics, analyzerIds);

            Environment.Exit(0);
        }

        private static async Task<List<Diagnostic>> AnalyzeSolution(List<Diagnostic> diagnostics, ConcurrentBag<ProjectSummary> projectSummaries, ImmutableArray<DiagnosticAnalyzer> analyzers, Dictionary<string, string?> severityConfig, Compilation?[] compilations)
        {
            var analysis = compilations.Where(c => c != null)
                                .Select(c => ProcessCompilation(c, analyzers, projectSummaries, severityConfig)).ToArray();
            return (await Task.WhenAll(analysis)).SelectMany(x => x).ToList();
        }

        private static void SaveOrPrintDiagnosticsCSV(ConsoleProgramOptions options, List<Diagnostic> diagnostics, Dictionary<string, string?> severityConfig)
        {
            var diagnosticPrinter = new Printer(options.Output);
            var CSVDiagnostics = from d in diagnostics select DiagnosticToCSV(d, d.GetSeverityWithConfig(severityConfig));
            diagnosticPrinter.Print("File Path, Start Line, Start Character, Severity, ID, Message");
            diagnosticPrinter.Print(CSVDiagnostics);
        }

        private static void PrintProjectSummaries(ConcurrentBag<ProjectSummary> projectSummaries)
        {
            var sortedSummaries = projectSummaries.OrderByDescending(s => s.FoundSmells).ThenByDescending(s => s.HiddenSmells);

            var totalTestClasses = sortedSummaries.Select(s => s.TestClasses).Sum();
            var totalTestMethods = sortedSummaries.Select(s => s.TestMethods).Sum();
            var totalFoundSmells = sortedSummaries.Select(s => s.FoundSmells).Sum();
            var totalHiddenSmells = sortedSummaries.Select(s => s.HiddenSmells).Sum();

            foreach (var summary in sortedSummaries)
            {
                System.Console.WriteLine(summary.ToString());
            }
            System.Console.WriteLine($"Solution Summary: {totalTestClasses} test classes, {totalTestMethods} test methods, {totalFoundSmells} diagnostics ({totalHiddenSmells} hidden)");
        }

        private static Dictionary<string, string?> SetConfig(ConsoleProgramOptions options, string[] analyzerIds)
        {
            string? configJson = GetOptionalEditorJson(options);

            if (configJson is null)
            {
                return new();
            }
            else
            {
                using var configValues = JsonDocument.Parse(configJson);
                var root = configValues.RootElement.EnumerateObject();
                var generalSettings = root.Where(e => !e.NameEquals("severity")).ToDictionary(e => e.Name, e => e.Value.GetString());
                SettingSingleton.SetSettings(generalSettings, options.IgnoreExistingConfig);


                if (configValues.RootElement.TryGetProperty("severity", out var severitiesProperty))
                {
                    try
                    {
                        return severitiesProperty.Deserialize<Dictionary<string, string>>();
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine(ex.Message);
                        Environment.Exit(0);
                        return new();
                    }

                }
                else
                {
                    return new();
                }
            }
        }

        private static async Task<Diagnostic[]> ProcessCompilation(
            Compilation compilation,
            ImmutableArray<DiagnosticAnalyzer> analyzers,
            ConcurrentBag<ProjectSummary> projectSummariesBag,
            Dictionary<string, string?> severityConfig)
        {
            var testClassAttr = compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute");
            var testMethodAttr = compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute");
            if ((testClassAttr is not null) && (testMethodAttr is not null))
            {

                var analyzerTask = compilation.WithAnalyzers(analyzers).GetAnalyzerDiagnosticsAsync();

                CountTests(compilation, testClassAttr, testMethodAttr, out int testClassAmount, out int testMethodAmount);

                var analyzerResults = await analyzerTask;
                var relevantResults = analyzerResults.Where(d => d.GetSeverityWithConfig(severityConfig) != DiagnosticSeverity.Hidden && d.Id != "AD0001");
                var exceptions = analyzerResults.Where(d => d.Id == "AD0001");


                System.Console.ForegroundColor = ConsoleColor.Red;
                foreach (var e in exceptions)
                {
                    System.Console.WriteLine(e + "\n");
                }
                System.Console.ResetColor();

                var foundSmells = relevantResults.Count();
                var hiddenSmells = analyzerResults.Length - relevantResults.Count();

                var projectSummary = new ProjectSummary(foundSmells, hiddenSmells, testClassAmount, testMethodAmount,
                    $"\t{compilation.AssemblyName}: {testClassAmount} test classes, {testMethodAmount} test methods, {foundSmells} diagnostics ({hiddenSmells} hidden)");

                projectSummariesBag.Add(projectSummary);


                return relevantResults.ToArray();

            }
            else
            {
                var projectSummary = new ProjectSummary(0, 0, 0, 0, $"\t{compilation.AssemblyName}: Project has no tests");
                projectSummariesBag.Add(projectSummary);

                return Array.Empty<Diagnostic>();
            }
        }



        private static void SaveMethodSummaryCSV(ConsoleProgramOptions options, List<Diagnostic> diagnostics, string[] analyzerIds)
        {
            if (options.OutputMethods is not null)
            {
                System.Console.WriteLine($"Printing method summary details to {options.OutputMethods}");

                var methodPrinter = new Printer(options.OutputMethods);
                Dictionary<string, int> idCounter() => analyzerIds.ToDictionary(id => id, id => 0);

                (string method, string id)[] methodDiagnostics = diagnostics
                    .Select(d => (d.Properties.TryGetValue("MethodName", out var value) ? (value ?? "None") : "None", d.Id)).ToArray();

                var methodCounter = methodDiagnostics.Select(d => (d.method)).ToHashSet().ToDictionary(m => m, m => idCounter());

                foreach (var (method, id) in methodDiagnostics)
                {
                    methodCounter[method][id]++;
                }

                List<string> lines = new() { $"Method, {string.Join(", ", analyzerIds)}, Total diagnostics" };

                List<(string line, int total)> values = new();
                foreach ((var method, var diagnosticAmounts) in methodCounter)
                {
                    var orderedAmount = diagnosticAmounts.OrderBy(p => p.Key).Select(p => p.Value);
                    values.Add(($"{method}, {string.Join(", ", orderedAmount)}, {orderedAmount.Sum()}", orderedAmount.Sum()));
                }
                lines.AddRange(values.OrderByDescending(v => v.total).Select(v => v.line));

                methodPrinter.Print(lines.ToArray());

            }
        }

        private static ImmutableArray<DiagnosticAnalyzer> TestSmellAnalyzers()
        {
            DiagnosticAnalyzer[] a =
            {
                //AssertionRoulette
                //ConditionalTest
                //EmptyTest
                //ExceptionHandling
                //IgnoredTest
                //MagicNumber
                //RedundantAssertion
                //SleepyTest
                //Mystery Guest
                //Duplicate Assert
                //Unknown Test
                new Compendium.AnalyzerCompendium(),

                new EagerTest.EagerTestAnalyzer(),
                new GeneralFixture.GeneralFixtureAnalyzer(),

            };
            return ImmutableArray.Create(a);
        }
        private static IEnumerable<INamedTypeSymbol> GetTestClassSymbols(INamespaceSymbol Namespace, INamedTypeSymbol testClassAttr)
        {
            var stack = new Stack<INamespaceSymbol>();
            stack.Push(Namespace);

            while (stack.Count > 0)
            {
                var @namespace = stack.Pop();

                foreach (var member in @namespace.GetMembers())
                {
                    if (member is INamespaceSymbol memberAsNamespace)
                    {
                        stack.Push(memberAsNamespace);
                    }
                    else if (member is INamedTypeSymbol memberAsNamedTypeSymbol)
                    {
                        if (TestUtils.AttributeIsInSymbol(testClassAttr, memberAsNamedTypeSymbol))
                        {
                            yield return memberAsNamedTypeSymbol;
                        }
                    }
                }
            }
        }
        private static string? GetOptionalEditorJson(ConsoleProgramOptions options)
        {
            if (options.Config is not null)
            {
                try
                {
                    return File.ReadAllText(options.Config);
                }
                catch (FileNotFoundException fnf)
                {
                    System.Console.WriteLine(fnf.Message);
                    Environment.Exit(0);
                }
            }
            return null;
        }

        private static string[] SupportedDagnosticsIds(ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            return analyzers.SelectMany(a => a.SupportedDiagnostics).Select(d => d.Id).Order().ToArray();
        }

        static string DiagnosticToCSV(Diagnostic d, DiagnosticSeverity diagnosticSeverity)
        {

            return DiagnosticCSVFormatter.Instance.Format(d, diagnosticSeverity);// d.ToString(); //String.Join(", ", d.Location, d.Id, d.WarningLevel);
        }

        static async Task<Solution> TryOpenSolutionAsync(MSBuildWorkspace workspace, string solutionName)
        {
            try
            {
                return await workspace.OpenSolutionAsync(solutionName);
            }
            catch (FileNotFoundException fnf)
            {
                System.Console.WriteLine(fnf.Message);
                Environment.Exit(1);
            }
            return null;
        }

        private static DiagnosticSeverity GetSeverityWithConfig(this Diagnostic diagnostic, Dictionary<string, string?> config)
        {
            if (config.TryGetValue(diagnostic.Id, out var severityString))
            {
                var severity = StringToSeverity(severityString);
                return severity ?? diagnostic.Severity;
            }
            else return diagnostic.Severity;
        }

        private static DiagnosticSeverity? StringToSeverity(string? severityString)
        {
            switch (severityString)
            {
                case "error":
                    return DiagnosticSeverity.Error;
                case "warning":
                    return DiagnosticSeverity.Warning;
                case "suggestion":
                    return DiagnosticSeverity.Info;
                case "silent":
                    return DiagnosticSeverity.Hidden;
                default:
                    return null;
            }
        }



        private static void CountTests(Compilation compilation, INamedTypeSymbol? testClassAttr, INamedTypeSymbol? testMethodAttr, out int testClassAmount, out int testMethodAmount)
        {
            var testClasses = GetTestClassSymbols(compilation.SourceModule.GlobalNamespace, testClassAttr);
            testClassAmount = testClasses.Count();
            testMethodAmount = 0;
            foreach (var testClass in testClasses)
            {
                var testMethods = testClass.GetMembers().Where(t => TestUtils.AttributeIsInSymbol(testMethodAttr, t));
                testMethodAmount += testMethods.Count();
            }
        }
    }
}