using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Immutable;
using System.Linq;


namespace TestSmells.Compendium.ExceptionHandling
{
    public class ExceptionHandlingAnalyzer
    {
        public const string DiagnosticId = "ExceptionHandling";


        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Test Smells";

        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        internal static Action<OperationAnalysisContext> AnalyzeOperations(string description)
        {
            return (context) =>
            {
                var operation = context.Operation;
                var diagnostic = Diagnostic.Create(Rule, operation.Syntax.GetLocation(), properties: TestUtils.MethodNameProperty(context), context.ContainingSymbol.Name, description);
                context.ReportDiagnostic(diagnostic);
            };
        }


    }
}
