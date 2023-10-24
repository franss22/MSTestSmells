using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
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

        internal static Action<SyntaxNodeAnalysisContext> AnalyzeInvocation(ISymbol[] assertionMethods)
        {
            return (context) =>
            {
                var invocationExpr = (InvocationExpressionSyntax)context.Node;
                var memberAccessExpr = invocationExpr.Expression as MemberAccessExpressionSyntax;
                if (memberAccessExpr is null) return;

                var memberSymbol = context.SemanticModel.GetSymbolInfo(memberAccessExpr).Symbol as IMethodSymbol;
                if (!TestUtils.MethodIsInList(memberSymbol, assertionMethods)) return;

                var argumentList = invocationExpr.ArgumentList;
                if ((argumentList?.Arguments.Count ?? 0) < 2) return;

                var expectedArg = argumentList.Arguments[0];
                var actualArg = argumentList.Arguments[1];

                if (ArgumentIsNumericLiteral(expectedArg))
                {
                    var diagnosticExpected = Diagnostic.Create(Rule, expectedArg.GetLocation(), memberAccessExpr.Name, expectedArg.ToString());
                    context.ReportDiagnostic(diagnosticExpected);
                }
                if (ArgumentIsNumericLiteral(actualArg))
                {
                    var diagnosticActual = Diagnostic.Create(Rule, actualArg.GetLocation(), memberAccessExpr.Name, actualArg.ToString());
                    context.ReportDiagnostic(diagnosticActual);
                }
            };


        }

        private static bool ArgumentIsNumericLiteral(ArgumentSyntax arg)
        {
            //Checks if the given expression is a numeric literal, or a cast numeric literal
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
