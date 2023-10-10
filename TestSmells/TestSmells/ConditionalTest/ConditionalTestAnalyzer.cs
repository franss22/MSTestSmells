using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;


namespace TestSmells.ConditionalTest
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConditionalTestAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ConditionalTest";


        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Test Smells";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(FindTestingClass);
        }

        private static void FindTestingClass(CompilationStartAnalysisContext context)
        {

            // Get the attribute object from the compilation
            var testClassAttr = context.Compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute");
            if (testClassAttr is null) { return; }
            var testMethodAttr = context.Compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute");
            if (testMethodAttr is null) { return; }



            // We register a Symbol Start Action to filter all test classes and their test methods
            context.RegisterSymbolStartAction((ctx) =>
            {
                if (!TestUtils.TestMethodInTestClass(ctx, testClassAttr, testMethodAttr)) { return; }
                ctx.RegisterOperationAction(AnalyzeConditionalOperations, OperationKind.Conditional, OperationKind.Loop, OperationKind.Switch);

            }
            , SymbolKind.Method);
        }

        private static void AnalyzeConditionalOperations(OperationAnalysisContext context)
        {
            var operation = context.Operation;
            string controlType;
            var methodName = context.ContainingSymbol.Name;
            if (operation.Kind == OperationKind.Conditional) 
            {
                controlType = "conditional";
            }
            else if (operation.Kind == OperationKind.Loop)
            {
                controlType = "loop";
            }
            else if (operation.Kind == OperationKind.Switch)
            {
                controlType= "switch";
            }
            else { return; }
            var diagnostic = Diagnostic.Create(Rule, operation.Syntax.GetLocation(), methodName, controlType);
            context.ReportDiagnostic(diagnostic);

        }
    }
}
