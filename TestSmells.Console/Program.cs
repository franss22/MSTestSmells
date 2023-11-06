using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using System.Collections.Immutable;
using CommandLine;
using System.IO;

namespace TestSmells.Console
{
    internal class Printer
    {
        private TextWriter WriteOut;

        public Printer(string? outputPath = null)
        {
            if (outputPath == null) 
            {
                WriteOut = System.Console.Out;
            }
            else
            {
                try
                {
                    if (File.Exists(outputPath))
                    {
                        File.Delete(outputPath);
                    }
                    FileStream fs = File.Create(outputPath);
                    var writer = new StreamWriter(fs);
                    //writer.AutoFlush = true;
                    WriteOut = writer;
                    System.Console.WriteLine($"Writing to {Path.GetFullPath(outputPath)}");
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    Environment.Exit(1);
                }
            }
        }


        public void Print(IEnumerable<object> values)
        {
            foreach (var value in values)
            {
                WriteOut.WriteLine(value);
            }
            WriteOut.Flush();
        }

        public void Print(string value)
        {

            WriteOut.WriteLine(value);
            WriteOut.Flush();
        }



    }

    internal static class Program
    {
        private class Options
        {
            [Option('s', "solution", Required = true, HelpText = "Absolute path to solution to be analyzed.")]
            public string Solution { get; set; }

            [Option('o', "output", Required = false, Default = null, HelpText = "File path for csv output. If left empty, results are printed in stdout")]
            public string? Output { get; set; }

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


        //const string slnPath = @"C:\Repos\TestSmellsMemoria\TestSmells.sln";
        //const string slnPath = @"C:\Users\frano\source\repos\TestProject1\TestProject1.sln";

        static async Task Main(string[] args)
        {
            var result = await Parser.Default.ParseArguments<Options>(args)
                .WithParsedAsync(RunOptions);
        }

        static ImmutableArray<DiagnosticAnalyzer> TestSmellAnalyzers()
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
                new MysteryGuest.MysteryGuestAnalyzer(),
                new UnknownTest.UnknownTestAnalyzer(),
            };
            return ImmutableArray.Create(a);
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
            var printer = new Printer(options.Output);
            var diagnostics = new List<Diagnostic>();


            System.Console.WriteLine("Loading analyzers");

            MSBuildLocator.RegisterDefaults();
            using (var workspace = MSBuildWorkspace.Create())
            {
                System.Console.WriteLine("Loading solution");

                var solution = await TryOpenSolutionAsync(workspace, options.Solution);

                System.Console.WriteLine("Compiling Solution");
                var compilations = await Task.WhenAll(solution.Projects
                    .Select(p => p.GetCompilationAsync()));

                System.Console.WriteLine("Analyzing Solution");

                var analysis = compilations.Where(c => c != null).Select(c => AnalyzeCompilation(c)).ToArray();

                var results = (await Task.WhenAll(analysis)).SelectMany(x => x).ToArray();
                diagnostics.AddRange(results);


                var CSVDiagnostics = from d in diagnostics select DiagnosticToCSV(d);

                printer.Print("File Path, Start Line, Start Character, Severity, ID, Message");
                printer.Print(CSVDiagnostics);

            }
            Environment.Exit(0);
        }

        private static async Task<Diagnostic[]> AnalyzeCompilation(Compilation compilation)
        {
            var testClassAttr = compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute");
            var testMethodAttr = compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute");
            if (!(testClassAttr is null) && !(testMethodAttr is null))
            {
                var analyzerTask = compilation.WithAnalyzers(TestSmellAnalyzers()).GetAnalyzerDiagnosticsAsync();

                var testClasses = GetTestClassSymbols(compilation.SourceModule.GlobalNamespace, testClassAttr);
                var testClassAmount = testClasses.Count();
                var testMethodAmount = 0;

                foreach (var testClass in testClasses)
                {
                    var testMethods = testClass.GetMembers().Where(t => TestUtils.AttributeIsInSymbol(testMethodAttr, t));
                    testMethodAmount += testMethods.Count();
                }

                var analyzerResults = await analyzerTask;
                var relevantResults = analyzerResults.Where(d => d.Severity != DiagnosticSeverity.Hidden);

                System.Console.WriteLine($"{compilation.AssemblyName}: {testClassAmount} test classes, {testMethodAmount} test methods, {relevantResults.Count()} diagnostics ({analyzerResults.Count() - relevantResults.Count()} hidden)");

                return relevantResults.ToArray();

            }
            else
            {
                System.Console.WriteLine($"{compilation.AssemblyName}: Project has no tests");
                return new Diagnostic[0];
            }
        }
    }
}