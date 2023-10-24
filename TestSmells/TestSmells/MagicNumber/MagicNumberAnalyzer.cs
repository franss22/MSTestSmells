using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;


namespace TestSmells.MagicNumber
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MagicNumberAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "MagicNumber";


        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Test Smells";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        private static readonly string[] RelevantAssertionsNames = { "AreEqual", "AreNotEqual"};


        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction(FindRelevantAssertions);
        }

        private void FindRelevantAssertions(CompilationStartAnalysisContext context)
        {
            var relevantAssertions = TestUtils.GetAssertionMethodSymbols(context.Compilation, RelevantAssertionsNames);
            if (relevantAssertions.Length == 0) return;

            var analyzeNode = AnalyzeNode(relevantAssertions);

            context.RegisterSyntaxNodeAction(analyzeNode, SyntaxKind.InvocationExpression);

        }

        private static Action<SyntaxNodeAnalysisContext> AnalyzeNode(ISymbol[] relevantAssertions)
        {
            return (SyntaxNodeAnalysisContext context) =>
            {
                var invocationExpr = (InvocationExpressionSyntax)context.Node;
                var memberAccessExpr = invocationExpr.Expression as MemberAccessExpressionSyntax;
                if (memberAccessExpr is null) return;
                //if (!RelevantAssertions.Contains(memberAccessExpr.Name.ToString())) return;

                var memberSymbol = context.SemanticModel.GetSymbolInfo(memberAccessExpr).Symbol as IMethodSymbol;
                if (!TestUtils.MethodIsInList(memberSymbol, relevantAssertions)) return;

                var argumentList = invocationExpr.ArgumentList as ArgumentListSyntax;
                if ((argumentList?.Arguments.Count ?? 0) < 2) return;

                var expectedArg = argumentList.Arguments[0] as ArgumentSyntax;
                var actualArg = argumentList.Arguments[1] as ArgumentSyntax;

                if (ArgumentIsNumericLiteral(expectedArg))
                {
                    //raise diagnostic
                    var diagnosticExpected = Diagnostic.Create(Rule, expectedArg.GetLocation(), memberAccessExpr.Name, expectedArg.ToString());
                    context.ReportDiagnostic(diagnosticExpected);
                }
                if (ArgumentIsNumericLiteral(actualArg))
                {
                    //raise diagnostic
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
