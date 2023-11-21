using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace TestSmells.Compendium.RedundantAssertion
{
    public class RedundantAssertionAnalyzer
    {
        public const string DiagnosticId = "RedundantAssertion";


        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Test Smells";

        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        private static readonly string[] SmellSpecificAssertionMethodNames = {
            //Assert
            "AreEqual",//expected, actual
            "AreNotEqual",//notExpected, actual
            "AreNotSame",//notExpected, actual
            "AreSame",//expected, actual
            //CollectionAssert
            "AreEqual",//expected, actual
            "AreEquivalent",//expected, actual
            "AreNotEqual",//notExpected, actual
            "AreNotEquivalent",//expected, actual
            "IsNotSubsetOf",//subset, superset
            "IsSubsetOf",//subset, superset
            //StringAssert
            "Contains",//value, substring
            "EndsWith",//value, substring
            "StartsWith",//value, substring
        };

        private static readonly string[] ArgNames =
            {
            "expected",
            "actual",
            "notExpected",
            "subset",
            "superset",
            "value",
            "substring"
        };

        internal static IMethodSymbol[] RelevantAssertions(Compilation compilation)
        {
            return TestUtils.GetAssertionMethodSymbols(compilation, SmellSpecificAssertionMethodNames);
        }


        internal static Action<OperationAnalysisContext> AnalyzeInvocations(IEnumerable<IMethodSymbol> assertionSymbols)
        {
            return (OperationAnalysisContext context) =>
            {
                var invocation = (IInvocationOperation)context.Operation;
                if (TestUtils.MethodIsInList(invocation.TargetMethod, assertionSymbols))
                {
                    var relevantArguments = invocation.Arguments.Where(arg => ArgNames.Contains(arg.Parameter.Name)).ToArray();
                    if (relevantArguments.Count() == 2 && AreSimilarArguments(relevantArguments[0], relevantArguments[1]))
                    {
                        var diagnostic = Diagnostic.Create(Rule, invocation.Syntax.GetLocation(), properties: TestUtils.MethodNameProperty(context), context.ContainingSymbol.Name, invocation.TargetMethod.Name);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            };

        }

        private static bool AreSimilarArguments(IArgumentOperation argument1, IArgumentOperation argument2)
        {
            return argument1.Syntax.IsEquivalentTo(argument2.Syntax, true);
        }
    }
}
