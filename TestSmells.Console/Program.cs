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

namespace TestSmells.Console
{
    
    internal static class Program
    {
        private class Options
        {
            [Option('s', "solution", Required = true, HelpText = "Absolute path to solution to be analyzed.")]
            public string Solution { get; set; }

            [Option('o', "output", Required = false, Default = null, HelpText = "File path for diagnostic csv output. If left empty, results are printed in stdout")]
            public string? Output { get; set; }

            [Option('m', "method_output", Required = false, Default = null, HelpText = "File path for method csv output. If left empty, results are not printed")]
            public string? OutputMethods { get; set;}

            //[Option('c', "config", Required = false, Default = null, HelpText = "File path for global editorconfig file")]
            //public string? Config { get; set; }


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
            var result = await Parser.Default.ParseArguments<Options>(args)
                .WithParsedAsync(RunOptions);
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
                new Compendium.AnalyzerCompendium(),

                new DuplicateAssert.DuplicateAssertAnalyzer(),
                new EagerTest.EagerTestAnalyzer(),
                new GeneralFixture.GeneralFixtureAnalyzer(),
                new UnknownTest.UnknownTestAnalyzer(),
            };
            return ImmutableArray.Create(a);
        }

         private static string[] SupportedDagnosticsIds(ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            return analyzers.SelectMany(a => a.SupportedDiagnostics).Select(d => d.Id).Order().ToArray();
        }

        static string DiagnosticToCSV(Diagnostic d)
        {

            return DiagnosticCSVFormatter.Instance.Format(d);// d.ToString(); //String.Join(", ", d.Location, d.Id, d.WarningLevel);
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

        private static async Task RunOptions(Options options)
        {
            var diagnostics = new List<Diagnostic>();

            var projectSummaries = new ConcurrentBag<ProjectSummary>();

            var analyzers = TestSmellAnalyzers();
            var analyzerIds = SupportedDagnosticsIds(analyzers);

            System.Console.WriteLine("Loading analyzers");

            MSBuildLocator.RegisterDefaults();
            using (var workspace = MSBuildWorkspace.Create())
            {
                System.Console.WriteLine("Loading solution");
                var solution = await TryOpenSolutionAsync(workspace, options.Solution);

                //string? editorConfig = GetOptionalEditorConfig(options);
                //if (editorConfig != null)
                //{

                //    var configSource = SourceText.From(editorConfig);

                //    foreach (var projectId in solution.ProjectIds)
                //    {
                //        DocumentId documentId = DocumentId.CreateNewId(projectId, ".editorconfig_console");
                //        solution = solution.AddAnalyzerConfigDocument(documentId, ".editorconfig", configSource, filePath: "/.editorconfig");
                //    }

                //}   

                //workspace.TryApplyChanges(solution);


                System.Console.WriteLine("Compiling Solution");
                var compilations = await Task.WhenAll(solution.Projects.Select(p => p.GetCompilationAsync()));

                System.Console.WriteLine("Analyzing Solution");
                var analysis = compilations.Where(c => c != null).Select(c => ProcessCompilation(c, analyzers, projectSummaries)).ToArray();
                diagnostics = (await Task.WhenAll(analysis)).SelectMany(x => x).ToList();
            }

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


            System.Console.WriteLine($"Printing diagnostic details to {options.Output ?? "console"}");
            var diagnosticPrinter = new Printer(options.Output);
            var CSVDiagnostics = from d in diagnostics select DiagnosticToCSV(d);
            diagnosticPrinter.Print("File Path, Start Line, Start Character, Severity, ID, Message");
            diagnosticPrinter.Print(CSVDiagnostics);

            SaveMethodSummary(options, diagnostics, analyzerIds);

            Environment.Exit(0);
        }

        //private static string? GetOptionalEditorConfig(Options options)
        //{
        //    if (options.Config is not null)
        //    {
        //        try
        //        {
        //            return File.ReadAllText(options.Config);
        //        }
        //        catch (FileNotFoundException fnf)
        //        {
        //            System.Console.WriteLine(fnf.Message);
        //            Environment.Exit(0);
        //        }
        //    }
        //    return null;
        //}

        private static void SaveMethodSummary(Options options, List<Diagnostic> diagnostics, string[] analyzerIds)
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

                foreach ((var method, var diagnosticAmounts) in methodCounter)
                {
                    var orderedAmount = diagnosticAmounts.OrderBy(p => p.Key).Select(p => p.Value);
                    lines.Add($"{method}, {string.Join(", ", orderedAmount)}, {orderedAmount.Sum()}");
                }

                methodPrinter.Print(lines.ToArray());

            }
        }

        private static async Task<Diagnostic[]> ProcessCompilation(
            Compilation compilation,
            ImmutableArray<DiagnosticAnalyzer> analyzers,
            ConcurrentBag<ProjectSummary> projectSummariesBag)
        {
            var testClassAttr = compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute");
            var testMethodAttr = compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute");
            if ((testClassAttr is not null) && (testMethodAttr is not null))
            {

                var analyzerTask = compilation.WithAnalyzers(analyzers).GetAnalyzerDiagnosticsAsync();

                CountTests(compilation, testClassAttr, testMethodAttr, out int testClassAmount, out int testMethodAmount);

                var analyzerResults = await analyzerTask;
                var relevantResults = analyzerResults.Where(d => d.Severity != DiagnosticSeverity.Hidden);


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