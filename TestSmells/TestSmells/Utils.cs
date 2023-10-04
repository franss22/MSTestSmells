using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace TestSmells
{
    public class TestUtils
    {

        public static IBlockOperation GetBlockOperation(OperationBlockAnalysisContext context)
        {
            foreach (var block in context.OperationBlocks)//we look for the method body
            {
                if (block.Kind != OperationKind.Block) { continue; }
                var blockOperation = (IBlockOperation)block;
                return blockOperation;
            }
            return null;
        }

        public static bool FindAttributeInSymbol(INamedTypeSymbol attribute, ISymbol symbol)
        {
            foreach (var attr in symbol.GetAttributes())
            {
                if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass, attribute))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool TestMethodInTestClass(SymbolAnalysisContext context, INamedTypeSymbol testClassAttr, INamedTypeSymbol testMethodAttr)
        {
            var methodSymbol = (IMethodSymbol)context.Symbol;
            var containerClass = methodSymbol.ContainingSymbol;
            if (containerClass is null) { return false; }

            return FindAttributeInSymbol(testClassAttr, containerClass) && FindAttributeInSymbol(testMethodAttr, methodSymbol);

        }
        public static bool TestMethodInTestClass(SymbolStartAnalysisContext context, INamedTypeSymbol testClassAttr, INamedTypeSymbol testMethodAttr)
        {
            var methodSymbol = (IMethodSymbol)context.Symbol;
            var containerClass = methodSymbol.ContainingSymbol;
            if (containerClass is null) { return false; }

            return FindAttributeInSymbol(testClassAttr, containerClass) && FindAttributeInSymbol(testMethodAttr, methodSymbol);

        }
    }
    


}
