using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;

namespace TestSmells.IgnoredTest
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IgnoredTestAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "IgnoredTest";


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

            var ignoreAttr = context.Compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.IgnoreAttribute");
            if (ignoreAttr is null) { return; }

            context.RegisterSymbolAction((ctx) =>
            {
                if (!TestUtils.TestMethodInTestClass(ctx, testClassAttr, testMethodAttr)) { return; }
                var methodSymbol = ctx.Symbol;

                foreach (var attr in methodSymbol.GetAttributes())
                {
                    if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass, ignoreAttr))
                    {
                        ctx.ReportDiagnostic(Diagnostic.Create(Rule, attr.ApplicationSyntaxReference.GetSyntax().GetLocation(), methodSymbol.Name));
                    }
                }
            }
            , SymbolKind.Method);
        }

    }
}
