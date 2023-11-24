using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Immutable;
using System.Linq;


namespace TestSmells.Compendium.ConditionalTest
{
    public class ConditionalTestAnalyzer
    {
        public const string DiagnosticId = "ConditionalTest";


        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Test Smells";

        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        internal static void RegisterOperationActions(SymbolStartAnalysisContext symbolStartContext)
        {
            symbolStartContext.RegisterOperationAction(AnalyzeConditionalOperations("conditional"), OperationKind.Conditional);
            symbolStartContext.RegisterOperationAction(AnalyzeLoopOperations("loop"), OperationKind.Loop);
            symbolStartContext.RegisterOperationAction(AnalyzeConditionalOperations("switch"), OperationKind.Switch);
        }
        private static Action<OperationAnalysisContext> AnalyzeLoopOperations(string controlType)
        {
            return (context) =>
            {
                var loop = (ILoopOperation)context.Operation;
                if (loop.LoopKind == LoopKind.ForEach) return;

                var diagnostic = Diagnostic.Create(Rule, context.Operation.Syntax.GetLocation(), properties: TestUtils.MethodNameProperty(context), context.ContainingSymbol.Name, controlType);
                context.ReportDiagnostic(diagnostic);
            };
        }
        private static Action<OperationAnalysisContext> AnalyzeConditionalOperations(string controlType)
        {
            return (context) =>
            {
                var diagnostic = Diagnostic.Create(Rule, context.Operation.Syntax.GetLocation(), properties: TestUtils.MethodNameProperty(context), context.ContainingSymbol.Name, controlType);
                context.ReportDiagnostic(diagnostic);
            };
        }
    }
}
