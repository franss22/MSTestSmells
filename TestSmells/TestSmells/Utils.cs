using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Generic;

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
            foreach (var assertType in assertTypes)
            {
                if (!(assertType is null))
                {
                    foreach (var function in nameList)
                    {
                        foreach (var member in assertType.GetMembers(function))
                        {
                            assertionSymbols.Add((IMethodSymbol)member);
                        }
                    }
                }
            }

            return assertionSymbols.ToArray();
        }

        public static bool MethodIsInList(IMethodSymbol symbol, ISymbol[] relevantAssertions)
        {
            if (symbol == null) return false;

            foreach (var function in relevantAssertions)
            {
                if (SymbolEqualityComparer.Default.Equals(symbol.OriginalDefinition, function))
                {
                    return true;
                }
            }
            return false;

        }

    }
    


}
