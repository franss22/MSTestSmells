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

            var relevantAssertions = TestUtils.GetAssertionMethodSymbols(context.Compilation);
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

                var blockOperation = TestUtils.GetBlockOperation(context);
                if (blockOperation is null) { return; }

                foreach (var operation in blockOperation.Descendants())
                {
                    if ((operation.Kind == OperationKind.SimpleAssignment) || (operation.Kind == OperationKind.VariableDeclarator))
                    { 
                        assignments.Add(operation);
                        continue;
                    }

                    if (operation.Kind != OperationKind.Invocation) { continue; }
                    var invocationOperation = (IInvocationOperation)operation;
                    if (TestUtils.MethodIsInList(invocationOperation.TargetMethod, relevantAssertions))
                    {
                        assertions.Add(invocationOperation);
                    }

                }
                if (assertions.Count > 1)
                {
                    var calledMethods = new List<IMethodSymbol>();
                    var invocations = new List<IInvocationOperation>();
                    //var relatedAssertions = new List<IInvocationOperation>();
                    foreach (var assert in assertions)
                    {
                        var argValue = GetAssertionValueArgument(assert);

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
                    


                    var methodNamesSet = new HashSet<string>(from m in calledMethods select m.Name);
                    if (methodNamesSet.Count>1)
                    {
                        var firstLocation = assertions.First().Syntax.GetLocation();
                        var testLocation = context.OwningSymbol.Locations.First();

                        var secondaryLocations = new List<Location>(from o in assertions select o.Syntax.GetLocation());
                        secondaryLocations.Insert(0, testLocation);


                        var diagnostic = Diagnostic.Create(Rule, firstLocation, secondaryLocations, context.OwningSymbol.Name);
                        context.ReportDiagnostic(diagnostic);
                    }

                }
            };


        }




        private static IOperation GetAssertionValueArgument(IInvocationOperation assertion)
        {
            foreach (var argument in assertion.Arguments)
            {
                if (argument.Parameter is null) { continue; };
                if (!ParamNames.Contains(argument.Parameter.Name)) { continue; };
                return argument.Value;
            }
            return null;
        }

      
    }
}
