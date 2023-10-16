using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


namespace TestSmells.SleepyTest
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SleepyTestAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SleepyTest";


        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Test Smells";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            // Controls analysis of generated code (ex. EntityFramework Migration) None means generated code is not analyzed
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.EnableConcurrentExecution();

            //Registers callback to start analysis
            context.RegisterCompilationStartAction(FindTestingClass);
        }

        private static void FindTestingClass(CompilationStartAnalysisContext context)
        {

            // Get the attribute object from the compilation
            var testClassAttr = context.Compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute");
            if (testClassAttr is null) { return; }
            var testMethodAttr = context.Compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute");
            if (testMethodAttr is null) { return; }

            var threadClass = context.Compilation.GetTypeByMetadataName("System.Threading.Thread");
            if (threadClass is null) { return; }
            var threadSleep = new List<IMethodSymbol>( from m in threadClass.GetMembers("Sleep") select (IMethodSymbol)m);
            if (threadSleep.Count == 0 ) { return; }

            // We register a Symbol Start Action to filter all test classes and their test methods
            context.RegisterSymbolStartAction((ctx) =>
            {
                if (!TestUtils.TestMethodInTestClass(ctx, testClassAttr, testMethodAttr)) { return; }
                ctx.RegisterOperationAction(AnalyzeInvocationIOperation(threadSleep), OperationKind.Invocation);

            }
            , SymbolKind.Method);
        }

        private static Action<OperationAnalysisContext> AnalyzeInvocationIOperation(List<IMethodSymbol> threadSleep)
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
