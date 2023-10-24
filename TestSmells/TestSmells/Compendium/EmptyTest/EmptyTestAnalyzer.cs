using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;
using System.Linq;


namespace TestSmells.Compendium.EmptyTest
{
    public class EmptyTestAnalyzer
    {
        public const string DiagnosticId = "EmptyTest";

        //Defining localized names and info for the diagnostic
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Test Smells";

        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);


        internal static void AnalyzeMethodBlockIOperation(OperationBlockAnalysisContext context)
        {
            var block = TestUtils.GetBlockOperation(context);
            if (block is null) { return; }
            if (block.Descendants().Count() == 0)//if the method body has no operations, it is empty
            {
                var methodSymbol = context.OwningSymbol;
                var diagnostic = Diagnostic.Create(Rule, methodSymbol.Locations.First(), methodSymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}