using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;


namespace TestSmells.Compendium.MagicNumber
{
    public class MagicNumberAnalyzer
    {
        public const string DiagnosticId = "MagicNumber";


        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Test Smells";

        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);


        private static readonly string[] SmellSpecificAssertionMethodNames = { "AreEqual", "AreNotEqual" };

        internal static IMethodSymbol[] RelevantAssertions(Compilation compilation)
        {
            return TestUtils.GetAssertionMethodSymbols(compilation, SmellSpecificAssertionMethodNames);
        }

        internal static Action<OperationAnalysisContext> AnalyzeInvocation(ISymbol[] assertionMethods)
        {
            return (OperationAnalysisContext context) =>
            {
                var invocation = (IInvocationOperation)context.Operation;
                var targetMethod = invocation.TargetMethod;

                if (!TestUtils.MethodIsInList(targetMethod, assertionMethods)) { return; }

                var argumentList = invocation.Arguments;
                //if (argumentList.Length < 2) return;

                var expectedArg = argumentList[0].Syntax;
                var actualArg = argumentList[1].Syntax;

                if (ArgumentIsNumericLiteral(expectedArg))
                {
                    var diagnosticExpected = Diagnostic.Create(Rule, expectedArg.GetLocation(), targetMethod.Name, expectedArg.ToString());
                    context.ReportDiagnostic(diagnosticExpected);
                }
                if (ArgumentIsNumericLiteral(actualArg))
                {
                    var diagnosticActual = Diagnostic.Create(Rule, actualArg.GetLocation(), targetMethod.Name, actualArg.ToString());
                    context.ReportDiagnostic(diagnosticActual);
                }
            };


        }

        private static bool ArgumentIsNumericLiteral(SyntaxNode node)
        {
            //Checks if the given expression is a numeric literal, or a cast numeric literal
            var arg = node as ArgumentSyntax;
            if ( arg is null)
            {
                return false;
            }
            var argExpr = arg.Expression;
            if (argExpr.Kind() == SyntaxKind.CastExpression)
            {
                var castExpr = (CastExpressionSyntax)argExpr;
                var valExpr = castExpr.Expression;
                return valExpr.Kind() == SyntaxKind.NumericLiteralExpression;
            }
            else
            {
                return argExpr.Kind() == SyntaxKind.NumericLiteralExpression;
            }
        }
    }
}
