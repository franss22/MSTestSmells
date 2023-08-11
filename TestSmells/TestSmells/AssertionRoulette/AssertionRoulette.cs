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
            "ThrowsException",//not working
            "ThrowsExceptionAsync",//not working
            "Fail",
        };

        private static readonly string[] MessageAssertionsNames =
        {
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<T>(T, T, string)",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual<T>(T, T, string, params object[])",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(object, object, string)",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(object, object, string, params object[])",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(float, float, float, string)",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(float, float, float, string, params object[])",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(double, double, double, string)",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(double, double, double, string, params object[])",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(string, string, bool, string)",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(string, string, bool, string, params object[])",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(string, string, bool, System.Globalization.CultureInfo, string)",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(string, string, bool, System.Globalization.CultureInfo, string, params object[])",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual<T>(T, T, string)",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual<T>(T, T, string, params object[])",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(object, object, string)",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(object, object, string, params object[])",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(float, float, float, string)",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(float, float, float, string, params object[])",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(double, double, double, string)",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(double, double, double, string, params object[])",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(string, string, bool, string)",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(string, string, bool, string, params object[])",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(string, string, bool, System.Globalization.CultureInfo, string)",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(string, string, bool, System.Globalization.CultureInfo, string, params object[])",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotSame(object, object, string)",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotSame(object, object, string, params object[])",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreSame(object, object, string)",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreSame(object, object, string, params object[])",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(bool, string)",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(bool, string, params object[])",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(object, string)",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(object, string, params object[])",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNull(object, string)",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNull(object, string, params object[])",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(bool, string)",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(bool, string, params object[])",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail(string)",
            "Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail(string, params object[])",

        };

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction(GetTestAttributes);
        }

        private void GetTestAttributes(CompilationStartAnalysisContext context)
        {
            // Get the attribute object from the compilation
            var testClassAttr = context.Compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute");
            if (testClassAttr is null) { return; }
            var testMethodAttr = context.Compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute");
            if (testMethodAttr is null) { return; }

            var relevantAssertions = GetRelevantAssertions(context.Compilation);
            if (relevantAssertions.Length == 0) return;

            //var text = new List<string>();
            //foreach (var item in relevantAssertions)
            //{
            //    text.Add(item.ToString());
            //}


            var analyzeMethod = AnalyzeMethodSymbol(testClassAttr, testMethodAttr, relevantAssertions);

            context.RegisterSymbolStartAction(analyzeMethod, SymbolKind.Method);

        }

        private static Action<SymbolStartAnalysisContext> AnalyzeMethodSymbol(INamedTypeSymbol testClassAttr, INamedTypeSymbol testMethodAttr , IMethodSymbol[] relevantAssertions)
        {
            return (SymbolStartAnalysisContext context) =>
            {
                var methodSymbol = (IMethodSymbol)context.Symbol;
                //Check if the container class is [TestClass], skip if it's not
                var containerClass = methodSymbol.ContainingSymbol;
                if (containerClass is null) { return; }
                if (!FindAttributeInSymbol(testClassAttr, containerClass)) { return; }


                if (FindAttributeInSymbol(testMethodAttr, methodSymbol)) 
                {
                    var operationBlockAnalisis = AnalyzeMethodOperations(relevantAssertions);
                    context.RegisterOperationBlockAction(operationBlockAnalisis); 
                }
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
                    foreach (var operation in blockOperation.Descendants())
                    {
                        if (operation.Kind != OperationKind.Invocation) { continue; }
                        var invocationOperation = (IInvocationOperation)operation;
                        if (MethodIsInList(invocationOperation.TargetMethod, relevantAssertions))
                        {
                            assertions.Add(invocationOperation);
                        }
                    }
                }
                if (assertions.Count>1)
                {
                    var messageAssertions = GetMessageAssertions(relevantAssertions);

                    foreach (var assert in assertions)
                    {
                        if (!MethodIsInList(assert.TargetMethod, messageAssertions))
                        {
                            var invocationSyntax = assert.Syntax;
                            var diagnostic = Diagnostic.Create(Rule, invocationSyntax.GetLocation(), assert.TargetMethod.Name);
                            context.ReportDiagnostic(diagnostic);
                        }
                        

                    }
                }
            };
        }


        private static IMethodSymbol[] GetRelevantAssertions(Compilation compilation)
        {
            //use getType*s* oto get partial classes and then get members of each of them
            var assertType = compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.Assert");
            //var a = assertType.GetMembers();
            //var text = new List<string>();
            //foreach (var item in a)
            //{
            //    text.Add(item.ToString());
            //}
            var relevantAssertions = new List<IMethodSymbol>();
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
            return relevantAssertions.ToArray();
        }

        private static IMethodSymbol[] GetMessageAssertions(IMethodSymbol[] relevantAssertions)
        {
            var messageAssertions = new List<IMethodSymbol>();
            foreach (var method in relevantAssertions)
            {
                if (MessageAssertionsNames.Contains(method.ToString()))
                {
                    messageAssertions.Add(method);
                }
            }
            return messageAssertions.ToArray();
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

        private static bool FindAttributeInSymbol(INamedTypeSymbol attribute, ISymbol symbol)
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
    }
}
