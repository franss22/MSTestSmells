using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using System.Collections.Immutable;
using System.Reflection;
using TestSmells;
namespace TestSmells.Console
{
    internal class Program
    {
        //const string slnPath = @"C:\Repos\TestSmellsMemoria\TestSmells.sln";
        const string slnPath = @"C:\Users\frano\source\repos\TestProject1\TestProject1.sln";

        static async Task Main(string[] args)
        {

            MSBuildLocator.RegisterDefaults();
            using (var workspace = MSBuildWorkspace.Create())
            {
                var solution = await workspace.OpenSolutionAsync(slnPath);

                var compilations = await Task.WhenAll(solution.Projects
                    .Select(p => p.GetCompilationAsync()));

                var analyzers = ImmutableArray.CreateBuilder<DiagnosticAnalyzer>();

                Assembly.GetAssembly(typeof(AssertionRoulette.AssertionRouletteAnalyzer))?
                        .GetTypes()
                        .Where(x => typeof(DiagnosticAnalyzer).IsAssignableFrom(x))
                        .Select(Activator.CreateInstance)
                        .Cast<DiagnosticAnalyzer>()
                        .ToList()
                        .ForEach(x => analyzers.Add(x));

                foreach (var compilation in compilations)
                {
                    if (compilation != null)
                    {

                        System.Console.WriteLine(compilation.AssemblyName);

                        var analyzerResults = await compilation.WithAnalyzers(analyzers.ToImmutable()).GetAnalyzerDiagnosticsAsync();

                        foreach (var analyzerResult in analyzerResults.Where(d => d.Severity != Microsoft.CodeAnalysis.DiagnosticSeverity.Hidden))
                        {
                            System.Console.WriteLine(analyzerResult);
                        }


                    }
                }
            }
        }
    }
}