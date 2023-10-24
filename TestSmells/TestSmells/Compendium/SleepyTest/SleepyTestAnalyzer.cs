using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


namespace TestSmells.Compendium.SleepyTest
{
    public class SleepyTestAnalyzer
    {
        public const string DiagnosticId = "SleepyTest";


        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Test Smells";

        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);



        internal static Action<OperationAnalysisContext> AnalyzeInvocation(List<IMethodSymbol> threadSleep)
        {
            return (OperationAnalysisContext context) =>
            {
                var invocation = (IInvocationOperation)context.Operation;
                var calledMethod = invocation.TargetMethod;
                if (TestUtils.MethodIsInList(calledMethod, threadSleep))
                {
                    var diagnostic = Diagnostic.Create(Rule, invocation.Syntax.GetLocation(), context.ContainingSymbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            };
        }
    }
}
