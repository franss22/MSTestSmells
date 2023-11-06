using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;

namespace TestSmells.Compendium.IgnoredTest
{
    public class IgnoredTestAnalyzer
    {
        public const string DiagnosticId = "IgnoredTest";


        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Test Smells";

        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);



        internal static Action<SymbolAnalysisContext> CheckMethodSymbol(INamedTypeSymbol ignoreAttr, INamedTypeSymbol testClassAttr, INamedTypeSymbol testMethodAttr)
        {
            return (context) =>
            {
                if (!TestUtils.TestMethodInTestClass(context, testClassAttr, testMethodAttr)) { return; }
                var methodSymbol = context.Symbol;

                //Done manually to get location of ignore attribute
                foreach (var attr in methodSymbol.GetAttributes())
                {
                    if (TestUtils.SymbolEquals(attr.AttributeClass, ignoreAttr))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, attr.ApplicationSyntaxReference.GetSyntax().GetLocation(), methodSymbol.Name));
                    }
                }
            };
        }


    }
}
