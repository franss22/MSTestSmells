using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


namespace TestSmells.RedundantAssertion
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RedundantAssertionAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "RedundantAssertion";


        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Test Smells";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

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

            var assertionMethods = TestUtils.GetAssertionMethodSymbols(context.Compilation, SmellSpecificAssertionMethodNames);

            // We register a Symbol Start Action to filter all test classes and their test methods
            context.RegisterSymbolStartAction((ctx) =>
            {
                if (!TestUtils.TestMethodInTestClass(ctx, testClassAttr, testMethodAttr)) { return; }
                ctx.RegisterOperationBlockAction(AnalyzeMethodBlockIOperation(assertionMethods));
            }
            , SymbolKind.Method);
        }

        private static Action<OperationBlockAnalysisContext> AnalyzeMethodBlockIOperation(IMethodSymbol[] assertionMethods)
        {
            return (OperationBlockAnalysisContext context) =>
            {
                var blockOperation = TestUtils.GetBlockOperation(context);
                if (blockOperation is null) { return; }
                var invocations = blockOperation.Descendants().Where(op => op.Kind == OperationKind.Invocation);
                var invocationOperations = new List<IInvocationOperation>(from op in invocations select (IInvocationOperation)op);
                var assertionInvocations = invocationOperations.Where(op => TestUtils.MethodIsInList(op.TargetMethod, assertionMethods));

                foreach (var assertInvocation in assertionInvocations)
                {
                    var relevantArguments = assertInvocation.Arguments.Where(arg => ArgNames.Contains(arg.Parameter.Name)).ToArray();
                    if (relevantArguments.Count()==2 && AreSimilarArguments(relevantArguments[0], relevantArguments[1])) 
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, assertInvocation.Syntax.GetLocation(), context.OwningSymbol.Name, assertInvocation.TargetMethod.Name));
                    }
                }
            };
        }

        private static bool AreSimilarArguments(IArgumentOperation invocation1, IArgumentOperation invocation2)
        {
            return (invocation1.Syntax.IsEquivalentTo(invocation2.Syntax, true));
        }
    }
}
