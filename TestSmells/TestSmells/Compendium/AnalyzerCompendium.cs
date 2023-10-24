using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using System;
using Microsoft.CodeAnalysis.Operations;
using System.Linq;
using System.Collections.Generic;

using TestSmells.Compendium.ConditionalTest;
using TestSmells.Compendium.EmptyTest;
using TestSmells.Compendium.ExceptionHandling;
using TestSmells.Compendium.MagicNumber;
using TestSmells.Compendium.IgnoredTest;
using TestSmells.Compendium.AssertionRoulette;
using TestSmells.Compendium.RedundantAssertion;
using TestSmells.Compendium.SleepyTest;

namespace TestSmells.Compendium
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]

    public class AnalyzerCompendium : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ListOfDiagnostics(); } }

        private ImmutableArray<DiagnosticDescriptor> ListOfDiagnostics()
        {

            DiagnosticDescriptor[] Rules =
            {
                EmptyTestAnalyzer.Rule,
                ExceptionHandlingAnalyzer.Rule,
                ConditionalTestAnalyzer.Rule,
                MagicNumberAnalyzer.Rule,
                IgnoredTestAnalyzer.Rule,
                RedundantAssertionAnalyzer.Rule,
                AssertionRouletteAnalyzer.Rule,
                SleepyTestAnalyzer.Rule,
            };
            return ImmutableArray.Create(Rules);
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction(CompilationAction);
        }
        private static void CompilationAction(CompilationStartAnalysisContext context)
        {

            // Get the attribute symbols from the compilation
            var testClassAttr = context.Compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute");
            if (testClassAttr is null) { return; }
            var testMethodAttr = context.Compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute");
            if (testMethodAttr is null) { return; }
            var ignoreAttr = context.Compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.IgnoreAttribute");
            if (ignoreAttr is null) { return; }

            var threadClass = context.Compilation.GetTypeByMetadataName("System.Threading.Thread");
            if (threadClass is null) { return; }
            var threadSleep = new List<IMethodSymbol>(from m in threadClass.GetMembers("Sleep") select (IMethodSymbol)m);
            if (threadSleep.Count == 0) { return; }

            var allAssertionMethods = TestUtils.GetAssertionMethodSymbols(context.Compilation);
            var magicNumberAssertions = MagicNumberAnalyzer.RelevantAssertions(context.Compilation);
            var redundantAssertionAssertions = RedundantAssertionAnalyzer.RelevantAssertions(context.Compilation);


            // We register a Symbol Start Action to filter all test classes and their test methods
            context.RegisterSymbolStartAction((ctx) =>
            {
                if (!TestUtils.TestMethodInTestClass(ctx, testClassAttr, testMethodAttr)) { return; }

                // Empty Test
                ctx.RegisterOperationBlockAction(EmptyTestAnalyzer.AnalyzeMethodBlockIOperation);

                //Exception Handling
                ctx.RegisterOperationAction(ExceptionHandlingAnalyzer.AnalyzeOperations("throws an exception"), OperationKind.Throw);
                ctx.RegisterOperationAction(ExceptionHandlingAnalyzer.AnalyzeOperations("handles exceptions"), OperationKind.Try);

                //Conditional Test
                ctx.RegisterOperationAction(ConditionalTestAnalyzer.AnalyzeConditionalOperations("conditional"), OperationKind.Conditional);
                ctx.RegisterOperationAction(ConditionalTestAnalyzer.AnalyzeConditionalOperations("loop"), OperationKind.Loop);
                ctx.RegisterOperationAction(ConditionalTestAnalyzer.AnalyzeConditionalOperations("switch"), OperationKind.Switch);

                //Magic Number
                ctx.RegisterSyntaxNodeAction(MagicNumberAnalyzer.AnalyzeInvocation(magicNumberAssertions), SyntaxKind.InvocationExpression);

                //Sleepy Test
                ctx.RegisterOperationAction(SleepyTestAnalyzer.AnalyzeInvocation(threadSleep), OperationKind.Invocation);


                //Assertion Roulette
                //Redundant Assertions
                ctx.RegisterOperationBlockAction(AssertionIterationAnalyzers(allAssertionMethods, redundantAssertionAssertions));

            }
            , SymbolKind.Method);

            //Ignored Test
            context.RegisterSymbolAction(IgnoredTestAnalyzer.CheckMethodSymbol(ignoreAttr, testClassAttr, testMethodAttr), SymbolKind.Method);
        }


        private static Action<OperationBlockAnalysisContext> AssertionIterationAnalyzers(IMethodSymbol[] allAssertions, IMethodSymbol[] redundantAssertionAssertions)
        {
            return (OperationBlockAnalysisContext context) =>
            {
                var block = TestUtils.GetBlockOperation(context);
                if (block is null) { return; }

                var assertionInvocations = block.Descendants()
                    .Where(op => op.Kind == OperationKind.Invocation)
                    .Cast<IInvocationOperation>()
                    .Where(invocation => TestUtils.MethodIsInList(invocation.TargetMethod, allAssertions));


                //Assertion Roulette
                AssertionRouletteAnalyzer.AnalyzeAssertions(context, assertionInvocations);

                //Redundant Assertion
                var redundantAssertionAssertionInvocations = assertionInvocations.Where(invocation => TestUtils.MethodIsInList(invocation.TargetMethod, redundantAssertionAssertions));
                RedundantAssertionAnalyzer.AnalyzeAssertions(context, redundantAssertionAssertionInvocations);


            };
            
        }

    }
}
