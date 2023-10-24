using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
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

        internal static void AnalyzeAssertions(OperationBlockAnalysisContext context, IEnumerable<IInvocationOperation> assertionInvocations)

        {

            if (assertionInvocations.Count() <= 1) { return; }

            var smellyAssertions = assertionInvocations.Where(invocation => !IsMessageAssertion(invocation.TargetMethod));

            foreach (var assert in smellyAssertions)
            {
                var invocationSyntax = assert.Syntax;
                if (invocationSyntax.IsKind(SyntaxKind.InvocationExpression))
                {
                    var diagnostic = Diagnostic.Create(Rule, invocationSyntax.GetLocation(), assert.TargetMethod.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }

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
