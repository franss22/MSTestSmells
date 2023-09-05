﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;
using System.Linq;


namespace TestSmells.EmptyTest
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EmptyTestAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "EmptyTest";

        //Defining localized names and info for the diagnostic
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



            // We register a Symbol Start Action to filter all test classes and their test methods
            context.RegisterSymbolStartAction((ctx) =>
            {
                if (!TestUtils.TestMethodInTestClass(ctx, testClassAttr, testMethodAttr)) { return; }
                ctx.RegisterOperationBlockAction(AnalyzeMethodBlockIOperation);

            }
            , SymbolKind.Method);
        }



        private static void AnalyzeMethodBlockIOperation(OperationBlockAnalysisContext context)
        {

            foreach (var block in context.OperationBlocks)//we look for the method body
            {
                if (block.Kind != OperationKind.Block) { continue; }
                if (block.Descendants().Count() == 0)//if the method body has no operations, it is empty
                {
                    var methodSymbol = context.OwningSymbol;
                    var diagnostic = Diagnostic.Create(Rule, methodSymbol.Locations.First(), methodSymbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }

            }

        }



    }
}