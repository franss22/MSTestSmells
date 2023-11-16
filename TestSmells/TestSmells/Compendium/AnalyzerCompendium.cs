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
using TestSmells.Compendium.MysteryGuest;
using TestSmells.Compendium.DuplicateAssert;
using TestSmells.Compendium.UnknownTest;

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
                MysteryGuestAnalyzer.Rule,
                DuplicateAssertAnalyzer.Rule,
                UnknownTestAnalyzer.Rule
            };
            return ImmutableArray.Create(Rules);
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(CompilationAction);
        }
        private static void CompilationAction(CompilationStartAnalysisContext compilationContext)
        {

            // Get the attribute symbols from the compilation
            var testClassAttr = compilationContext.Compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute");
            if (testClassAttr is null) { return; }
            var testMethodAttr = compilationContext.Compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute");
            if (testMethodAttr is null) { return; }


            var ignoreAttr = compilationContext.Compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.IgnoreAttribute");
            if (ignoreAttr is null) { return; }

            var threadClass = compilationContext.Compilation.GetTypeByMetadataName("System.Threading.Thread");
            if (threadClass is null) { return; }
            var threadSleep = new List<IMethodSymbol>(from m in threadClass.GetMembers("Sleep") select (IMethodSymbol)m);
            if (threadSleep.Count == 0) { return; }

            var allAssertionMethods = TestUtils.GetAssertionMethodSymbols(compilationContext.Compilation);
            var magicNumberAssertions = MagicNumberAnalyzer.RelevantAssertions(compilationContext.Compilation);
            var redundantAssertionAssertions = RedundantAssertionAnalyzer.RelevantAssertions(compilationContext.Compilation);
            var fileSymbols = new MysteryGuestAnalyzer.FileSymbols(compilationContext.Compilation);

            // We register a Symbol Start Action to filter all test classes and their test methods
            compilationContext.RegisterSymbolStartAction((symbolStartContext) =>
            {
                if (!TestUtils.TestMethodInTestClass(symbolStartContext, testClassAttr, testMethodAttr)) { return; }

                // Empty Test
                symbolStartContext.RegisterOperationAction(EmptyTestAnalyzer.AnalyzeMethodBodyOperation, OperationKind.MethodBody);

                //Exception Handling
                symbolStartContext.RegisterOperationAction(ExceptionHandlingAnalyzer.AnalyzeOperations("throws an exception"), OperationKind.Throw);
                symbolStartContext.RegisterOperationAction(ExceptionHandlingAnalyzer.AnalyzeOperations("handles exceptions"), OperationKind.Try);

                //Conditional Test
                symbolStartContext.RegisterOperationAction(ConditionalTestAnalyzer.AnalyzeConditionalOperations("conditional"), OperationKind.Conditional);
                symbolStartContext.RegisterOperationAction(ConditionalTestAnalyzer.AnalyzeConditionalOperations("loop"), OperationKind.Loop);
                symbolStartContext.RegisterOperationAction(ConditionalTestAnalyzer.AnalyzeConditionalOperations("switch"), OperationKind.Switch);

                //Magic Number
                symbolStartContext.RegisterOperationAction(MagicNumberAnalyzer.AnalyzeInvocation(magicNumberAssertions), OperationKind.Invocation);

                //Sleepy Test
                symbolStartContext.RegisterOperationAction(SleepyTestAnalyzer.AnalyzeInvocation(threadSleep), OperationKind.Invocation);

                //MysteryGuest
                symbolStartContext.RegisterOperationBlockAction(MysteryGuestAnalyzer.AnalyzeMethodOperations(fileSymbols));

                //Redundant Assertions
                symbolStartContext.RegisterOperationAction(RedundantAssertionAnalyzer.AnalyzeInvocations(redundantAssertionAssertions), OperationKind.Invocation);

                //Assertion Roulette
                AssertionRouletteAnalyzer.RegisterTwoPartAnalysis(symbolStartContext, allAssertionMethods);

                //Duplicate Assert
                DuplicateAssertAnalyzer.RegisterTwoPartAnalysis(symbolStartContext, allAssertionMethods);

                //Unknown Test
                UnknownTestAnalyzer.RegisterTwoPartAnalysis(symbolStartContext, allAssertionMethods);

            }
            , SymbolKind.Method);

            //Ignored Test
            compilationContext.RegisterSymbolAction(IgnoredTestAnalyzer.CheckMethodSymbol(ignoreAttr, testClassAttr, testMethodAttr), SymbolKind.Method);
        }

    }
}
