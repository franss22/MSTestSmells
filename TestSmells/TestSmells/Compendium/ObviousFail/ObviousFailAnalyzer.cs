using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Immutable;
using System.Linq;


namespace TestSmells.Compendium.ObviousFail
{
    public class ObviousFailAnalyzer
    {
        public const string DiagnosticId = "ObviousFail";


        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Test Smells";

        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        private static readonly string[] SmellSpecificAssertionMethodNames = { "IsTrue", "IsFalse" };

        internal static IMethodSymbol[] RelevantAssertions(Compilation compilation)
        {
            return TestUtils.GetAssertionMethodSymbols(compilation, SmellSpecificAssertionMethodNames);
        }

        internal static Action<OperationAnalysisContext> AnalyzeAssertions(ISymbol[] assertionMethods)
        {
            return (OperationAnalysisContext context) =>
            {
                var invocation = (IInvocationOperation)context.Operation;
                var targetMethod = invocation.TargetMethod;

                if (!TestUtils.MethodIsInList(targetMethod, assertionMethods)) { return; }

                var argumentList = invocation.Arguments;

                var boolArg = (ArgumentSyntax) argumentList[0].Syntax;

                if (
                    (targetMethod.Name == "IsTrue" && boolArg.Expression.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.FalseLiteralExpression))||
                    (targetMethod.Name == "IsFalse" && boolArg.Expression.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.TrueLiteralExpression))
                    )
                {
                    var diagnosticExpected = Diagnostic.Create(Rule, invocation.Syntax.GetLocation(), properties: TestUtils.MethodNameProperty(context), invocation.Syntax.ToString());
                    context.ReportDiagnostic(diagnosticExpected);
                }
            };
        }
    }
}
