using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


namespace TestSmells.EagerTest
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EagerTestAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "EagerTest";


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
            //"IsNotSubsetOf",
            //"IsSubsetOf",
            //StringAssert
            "Contains",//edge (String, String)
            "DoesNotMatch",
            "EndsWith",//edge (String, String)
            "Matches",
            "StartsWith",//edge (String, String)

        };
        private static readonly string[] ParamNames =
        {
            "actual",
            "value",
            "condition",
            "collection",

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

                var fileOptions = context.Options.AnalyzerConfigOptionsProvider.GetOptions(context.FilterTree);
                fileOptions.TryGetValue("dotnet_diagnostic.MysteryGuest.IgnoredFiles", out var DifferentMethodsThresholdOpt);
                int differentMethodsThreshold = Int32.TryParse(DifferentMethodsThresholdOpt, out var tempVal) ? tempVal : 1;

                var assignments = new List<IOperation>();
                var assertions = new List<IInvocationOperation>();
                foreach (var block in context.OperationBlocks)//we look for the method body
                {
                    if (block.Kind != OperationKind.Block) { continue; }
                    var blockOperation = (IBlockOperation)block;
                    var descendants = blockOperation.Descendants();
                    foreach (var operation in descendants)
                    {
                        if ((operation.Kind == OperationKind.SimpleAssignment) || (operation.Kind == OperationKind.VariableDeclarator))
                        { assignments.Add(operation); }
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
                    var calledMethods = new List<IMethodSymbol>();
                    var invocations = new List<IInvocationOperation>();
                    foreach (var assert in assertions)
                    {
                        foreach (var argument in assert.Arguments)
                        {
                            if (argument.Parameter is null) { continue; };
                            if (!ParamNames.Contains(argument.Parameter.Name)) { continue; };


                            var argValue = argument.Value;
                            if (argValue.Kind == OperationKind.Invocation)
                            {
                                var invocationArg = (IInvocationOperation)argValue;
                                calledMethods.Add(invocationArg.TargetMethod);
                                invocations.Add(invocationArg);
                            }

                            if (argValue.Kind == OperationKind.LocalReference)
                            {
                                var referenceArg = (ILocalReferenceOperation)argValue;
                                foreach (IOperation operation in assignments)
                                {
                                    if (operation.Kind == OperationKind.SimpleAssignment)
                                    {
                                        var assign = (ISimpleAssignmentOperation)operation;
                                        var target = (ILocalReferenceOperation)assign.Target;
                                        if (SymbolEqualityComparer.Default.Equals(target.Local, referenceArg.Local))
                                        {
                                            foreach (var op in assign.Value.DescendantsAndSelf())
                                            {
                                                if (op.Kind == OperationKind.Invocation)
                                                {
                                                    var invocation = (IInvocationOperation)op;
                                                    calledMethods.Add(invocation.TargetMethod);
                                                    invocations.Add(invocation);

                                                }
                                            }
                                            
                                        }

                                    }
                                    if (operation.Kind == OperationKind.VariableDeclarator)
                                    {
                                        var declaration = (IVariableDeclaratorOperation)operation;
                                        if (SymbolEqualityComparer.Default.Equals(declaration.Symbol, referenceArg.Local))
                                        {
                                            var init = declaration.Initializer;
                                            foreach (var op in init.Value.DescendantsAndSelf())
                                            {
                                                if (op.Kind == OperationKind.Invocation)
                                                {
                                                    var invocation = (IInvocationOperation)op;
                                                    calledMethods.Add(invocation.TargetMethod);
                                                    invocations.Add(invocation);

                                                }
                                            }

                                        }
                                    }
                                }

                            }
                        }



                    }

                    var methodNamesSet = new HashSet<string>(from m in calledMethods select m.Name);
                    if (methodNamesSet.Count>1)
                    {
                        var invocationLocations = new List<Location>();
                        foreach (var inv in invocations)
                        {
                            invocationLocations.Add(inv.Syntax.GetLocation());
                        }
                        var testLocation = context.OwningSymbol.Locations.First();
                        var diagnostic = Diagnostic.Create(Rule, testLocation, invocationLocations, context.OwningSymbol.Name);
                        context.ReportDiagnostic(diagnostic);
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
