using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestSmells
{
    public class TestUtils
    {
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
