using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace TestSmells.AssertionRoulette
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AssertionRouletteAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AssertionRoulette";


        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Test Smells";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        private static readonly string[] RelevantAssertionsNames = {
            //Assert
            "AreEqual",
            "AreNotEqual",
            "AreNotSame",
            "AreSame",
            "IsFalse",
            "IsInstanceOfType",
            "IsNotInstanceOfType",
            "IsNotNull",
            "IsNull",
            "IsTrue",
            "ThrowsException",
            "ThrowsExceptionAsync",
            "Fail",
            "Inconclusive",
            //CollectionAssert
            "AllItemsAreInstancesOfType",
            "AllItemsAreNotNull",
            "AllItemsAreUnique",
            "AreEqual",
            "AreEquivalent",
            "AreNotEqual",
            "AreNotEquivalent",
            "Contains",
            "DoesNotContain",
            "IsNotSubsetOf",
            "IsSubsetOf",
            //StringAssert
            "Contains",//edge (String, String)
            "DoesNotMatch",
            "EndsWith",//edge (String, String)
            "Matches",
            "StartsWith",//edge (String, String)

        };


        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction(GetClassesFromCompilation);
        }

        private void GetClassesFromCompilation(CompilationStartAnalysisContext context)
        {
            // Get the attribute object from the compilation
            var testClassAttr = context.Compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute");
            if (testClassAttr is null) { return; }
            var testMethodAttr = context.Compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute");
            if (testMethodAttr is null) { return; }

            var relevantAssertions = GetRelevantAssertions(context.Compilation);
            if (relevantAssertions.Length == 0) return;


            var analyzeMethod = AnalyzeMethodSymbol(testClassAttr, testMethodAttr, relevantAssertions);

            context.RegisterSymbolStartAction(analyzeMethod, SymbolKind.Method);

        }

        private static Action<SymbolStartAnalysisContext> AnalyzeMethodSymbol(INamedTypeSymbol testClassAttr, INamedTypeSymbol testMethodAttr, IMethodSymbol[] relevantAssertions)
        {
            return (SymbolStartAnalysisContext context) =>
            {
                if (!TestUtils.TestMethodInTestClass(context, testClassAttr, testMethodAttr)) { return; }

                var operationBlockAnalisis = AnalyzeMethodOperations(relevantAssertions);
                context.RegisterOperationBlockAction(operationBlockAnalisis);

            };


        }

        private static Action<OperationBlockAnalysisContext> AnalyzeMethodOperations(IMethodSymbol[] relevantAssertions)

        {
            return (OperationBlockAnalysisContext context) =>
            {
                var assertions = new List<IInvocationOperation>();
                foreach (var block in context.OperationBlocks)//we look for the method body
                {
                    if (block.Kind != OperationKind.Block) { continue; }
                    var blockOperation = (IBlockOperation)block;
                    var descendants = blockOperation.Descendants();
                    foreach (var operation in descendants)
                    {
                        if (operation.Kind != OperationKind.Invocation) { continue; }
                        var invocationOperation = (IInvocationOperation)operation;
                        if (MethodIsInList(invocationOperation.TargetMethod, relevantAssertions))
                        {
                            assertions.Add(invocationOperation);
                        }
                    }
                }
                if (assertions.Count > 1)
                {

                    foreach (var assert in assertions)
                    {
                        if (!IsMessageAssertion(assert.TargetMethod))
                        {
                            var invocationSyntax = assert.Syntax;
                            if (invocationSyntax.IsKind(SyntaxKind.InvocationExpression))
                            {
                                var diagnostic = Diagnostic.Create(Rule, invocationSyntax.GetLocation(), assert.TargetMethod.Name);
                                context.ReportDiagnostic(diagnostic);
                            }
                            
                        }
                    }
                }
            };
        }


        private static IMethodSymbol[] GetRelevantAssertions(Compilation compilation)
        {
            INamedTypeSymbol[] assertTypes = {
                compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.Assert"),
                compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.StringAssert"),
                compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.CollectionAssert"),
            };



            var relevantAssertions = new List<IMethodSymbol>();
            foreach (var assertType in assertTypes)
            {
                if (!(assertType is null))
                {
                    foreach (var function in RelevantAssertionsNames)
                    {
                        foreach (var member in assertType.GetMembers(function))
                        {
                            relevantAssertions.Add((IMethodSymbol)member);
                        }
                    }
                }
            }

            return relevantAssertions.ToArray();
        }

        private static bool IsMessageAssertion(IMethodSymbol method)
        {
            var args = method.OriginalDefinition.Parameters;
            if (args.Length == 0) return false;
            else if (args.Length == 1)
            {
                var lastArg = args[0];
                var lastArgName = lastArg.Name;
                return (lastArgName == "message");
            }
            else
            {
                var lastArg = args[args.Length - 1];
                var lastArgName = lastArg.Name;
                var scndLastArg = args[args.Length - 2];
                var scndLastArgName = scndLastArg.Name;
                return (lastArgName == "message" || (scndLastArgName == "message" && lastArgName == "parameters"));
            }

        }


        private static bool MethodIsInList(IMethodSymbol symbol, ISymbol[] relevantAssertions)
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
