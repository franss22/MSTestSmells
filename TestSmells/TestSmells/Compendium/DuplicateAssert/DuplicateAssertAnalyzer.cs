using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace TestSmells.Compendium.DuplicateAssert
{

    public class DuplicateAssertAnalyzer
    {
        public const string DiagnosticId = "DuplicateAssert";


        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Test Smells";

        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);


        internal static void RegisterTwoPartAnalysis(SymbolStartAnalysisContext context, IEnumerable<IMethodSymbol> assertionSymbols)
        {
            ConcurrentBag<IInvocationOperation> assertionBag = new ConcurrentBag<IInvocationOperation>();

            context.RegisterOperationAction(TestUtils.CollectAssertions(assertionSymbols, assertionBag), OperationKind.Invocation);
            context.RegisterSymbolEndAction(AnalyzeAssertions(assertionBag));
        }

        private static Action<SymbolAnalysisContext> AnalyzeAssertions(ConcurrentBag<IInvocationOperation> assertionBag)
        {
            return (context) =>
            {
                if (assertionBag.Count == 0) return;

                var assertions = assertionBag.OrderBy(a => a.Syntax.GetLocation().ToString()).ToArray();

                var similarInvocations = new List<List<IInvocationOperation>>
                {
                    new List<IInvocationOperation> { assertions.First() }
                };

                foreach (var assert in assertions.Skip(1))
                {
                    var duplicateList = similarInvocations.FirstOrDefault(group => AreSimilarInvocations(assert, group.First()));
                    if (duplicateList != null)
                    {
                        duplicateList.Add(assert);
                    }
                    else
                    {
                        similarInvocations.Add(new List<IInvocationOperation> { assert });
                    }
                }

                if (similarInvocations.Count == assertions.Count()) { return; }
                var testLocation = context.Symbol.Locations.First();
                var duplicateInvocations = similarInvocations.Where(l => l.Count > 1);
                foreach (var assertionGroup in duplicateInvocations)
                {

                    var locations = new List<Location>(from o in assertionGroup select o.Syntax.GetLocation());
                    var diagnosticLocation = locations.First();

                    var linesMessage = string.Join(", ", locations.Select(loc => loc.GetMappedLineSpan().Span.Start.Line + 1));

                    locations.Insert(0, testLocation);

                    var diagnostic = Diagnostic.Create(Rule, diagnosticLocation, locations, properties: TestUtils.MethodNameProperty(context), context.Symbol.Name, linesMessage);
                    context.ReportDiagnostic(diagnostic);

                }

            };
        }

        private static bool AreSimilarInvocations(IInvocationOperation invocation1, IInvocationOperation invocation2)
        {
            return invocation1.Syntax.IsEquivalentTo(invocation2.Syntax, true);
        }

    }
}
