using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace TestSmells.Compendium.AssertionRoulette
{
    public class AssertionRouletteAnalyzer
    {
        public const string DiagnosticId = "AssertionRoulette";


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

        internal static Action<SymbolAnalysisContext> AnalyzeAssertions(ConcurrentBag<IInvocationOperation> assertionInvocations)

        {
            return (SymbolAnalysisContext context) =>
            {
                if (assertionInvocations.Count() <= 1) { return; }

                var smellyAssertions = assertionInvocations.Where(invocation => !IsMessageAssertion(invocation.TargetMethod));

                foreach (var assert in smellyAssertions)
                {
                    var invocationSyntax = assert.Syntax;
                    if (invocationSyntax.IsKind(SyntaxKind.InvocationExpression))
                    {
                        var diagnostic = Diagnostic.Create(Rule, invocationSyntax.GetLocation(), properties: TestUtils.MethodNameProperty(context), assert.TargetMethod.Name);

                        context.ReportDiagnostic(diagnostic);
                    }
                }
            };
        }


        private static bool IsMessageAssertion(IMethodSymbol method)
        {
            var args = method.OriginalDefinition.Parameters;
            if (args.Length == 0) { return false; }
            else if (args.Length == 1)
            {
                var lastArg = args[0];
                var lastArgName = lastArg.Name;
                return lastArgName == "message";
            }
            else
            {
                var lastArg = args[args.Length - 1];
                var lastArgName = lastArg.Name;
                var scndLastArg = args[args.Length - 2];
                var scndLastArgName = scndLastArg.Name;
                return lastArgName == "message" || scndLastArgName == "message" && lastArgName == "parameters";
            }

        }

    }
}
