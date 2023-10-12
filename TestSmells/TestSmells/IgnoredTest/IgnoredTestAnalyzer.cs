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

            // We register a Symbol Start Action to filter all test classes and their test methods
            context.RegisterSymbolStartAction((ctx) =>
            {
                if (!TestUtils.TestMethodInTestClass(ctx, testClassAttr, testMethodAttr)) { return; }
                if (TestUtils.FindAttributeInSymbol(ignoreAttr, ctx.Symbol))
                {
                    ctx.RegisterSyntaxNodeAction(ReportIgnoreDiagnostic(ignoreAttr), SyntaxKind.MethodDeclaration);
                }

            }
            , SymbolKind.Method);
        }

        private static Action<SyntaxNodeAnalysisContext> ReportIgnoreDiagnostic(INamedTypeSymbol ignoreAttr)
        {

            return (SyntaxNodeAnalysisContext context) =>
            {
                var methodSyntax = (MethodDeclarationSyntax)context.Node;
                var attributeList = methodSyntax.AttributeLists.SelectMany(list => list.Attributes);

                
                foreach (var attribute in attributeList)
                {
                    var attributeSymbol = context.SemanticModel.GetSymbolInfo(attribute).Symbol;
                    if (attributeSymbol is null) { continue;}
                    var classSymbol = attributeSymbol.ContainingSymbol;
                    if (SymbolEqualityComparer.Default.Equals(ignoreAttr, classSymbol))
                    {
                        var diagnostic = Diagnostic.Create(Rule, attribute.GetLocation(), methodSyntax.Identifier.Text);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
                
            };
        }
    }
}
