using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace TestSmells
{
    public class TestUtils
    {
        public static readonly string[] AssertionMethodNames = {
            //Assert
            "AreEqual",//expected, actual
            "AreNotEqual",//notExpected, actual
            "AreNotSame",//notExpected, actual
            "AreSame",//expected, actual
            "Fail",
            "Inconclusive",
            "IsFalse",//condition
            "IsInstanceOfType",//value, expectedType
            "IsNotInstanceOfType",//value, wrongType
            "IsNotNull",//value
            "IsNull",//value
            "IsTrue",//condition
            "ThrowsException",//action
            "ThrowsExceptionAsync",//action
            //CollectionAssert
            "AllItemsAreInstancesOfType",//collection, expectedType
            "AllItemsAreNotNull",//collection
            "AllItemsAreUnique",//collection
            "AreEqual",//expected, actual
            "AreEquivalent",//expected, actual
            "AreNotEqual",//notExpected, actual
            "AreNotEquivalent",//expected, actual
            "Contains",//collection, element
            "DoesNotContain",//collection, element
            "IsNotSubsetOf",//subset, superset
            "IsSubsetOf",//subset, superset
            //StringAssert
            "Contains",//value, substring
            "DoesNotMatch",//value, pattern
            "EndsWith",//value, substring
            "Matches",//value, pattern
            "StartsWith",//value, substring
        };
        public static IBlockOperation GetBlockOperation(OperationBlockAnalysisContext context)
        {
            return (IBlockOperation)context.OperationBlocks.Where(op => op.Kind == OperationKind.Block).FirstOrDefault();
        }

        public static bool AttributeIsInSymbol(INamedTypeSymbol attribute, ISymbol symbol)
        {
            return symbol.GetAttributes().Any(attr => TestUtils.SymbolEquals(attr.AttributeClass, attribute));
        }

        public static bool TestMethodInTestClass(SymbolAnalysisContext context, INamedTypeSymbol testClassAttr, INamedTypeSymbol testMethodAttr)
        {
            var methodSymbol = (IMethodSymbol)context.Symbol;
            var containerClass = methodSymbol.ContainingSymbol;
            if (containerClass is null) { return false; }

            return AttributeIsInSymbol(testClassAttr, containerClass) && AttributeIsInSymbol(testMethodAttr, methodSymbol);

        }
        public static bool TestMethodInTestClass(SymbolStartAnalysisContext context, INamedTypeSymbol testClassAttr, INamedTypeSymbol testMethodAttr)
        {
            var methodSymbol = (IMethodSymbol)context.Symbol;
            var containerClass = methodSymbol.ContainingSymbol;
            if (containerClass is null) { return false; }

            return AttributeIsInSymbol(testClassAttr, containerClass) && AttributeIsInSymbol(testMethodAttr, methodSymbol);

        }

        public static IMethodSymbol[] GetAssertionMethodSymbols(Compilation compilation, string[] nameList = null)
        {
            INamedTypeSymbol[] assertTypes = {
                compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.Assert"),
                compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.StringAssert"),
                compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.CollectionAssert"),
            };

            if (nameList == null)
            {
                nameList = AssertionMethodNames;
            }

            var assertionSymbols = new List<IMethodSymbol>();
            foreach (var assertType in assertTypes.Where(a => a != null))
            {
                foreach (var function in nameList)
                {
                    assertionSymbols.AddRange(from m in assertType.GetMembers(function) select (IMethodSymbol)m);
                }
            }

            return assertionSymbols.ToArray();
        }

        public static bool MethodIsInList(IMethodSymbol symbol, IEnumerable<ISymbol> methodList)
        {
            if (symbol == null) return false;

            return methodList.Any(function => SymbolEquals(symbol.OriginalDefinition, function));

        }

        public static bool SymbolEquals(ISymbol s1, ISymbol s2)
        {
            return SymbolEqualityComparer.Default.Equals(s1, s2);
        }

        public static ImmutableDictionary<string, string> MethodNameProperty(SymbolAnalysisContext context)
        {
            return new Dictionary<string, string> { { "MethodName", context.Symbol.ToString() } }.ToImmutableDictionary();
        }

        public static ImmutableDictionary<string, string> MethodNameProperty(OperationBlockAnalysisContext context)
        {
            return new Dictionary<string, string> { { "MethodName", context.OwningSymbol.ToString() } }.ToImmutableDictionary();
        }

        public static ImmutableDictionary<string, string> MethodNameProperty(OperationAnalysisContext context)
        {
            return new Dictionary<string, string> { { "MethodName", context.ContainingSymbol.ToString() } }.ToImmutableDictionary();
        }

        public static ImmutableDictionary<string, string> ImmProperty(string key, string value)
        {
            return new Dictionary<string, string> { { key, value } }.ToImmutableDictionary();
        }

    }
}
