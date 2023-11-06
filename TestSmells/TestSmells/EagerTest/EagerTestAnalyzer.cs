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


        private static (IOperation[], IInvocationOperation[]) GetAssertionsAndAssignments(IBlockOperation blockOperation, IMethodSymbol[] relevantAssertions)
        {
            var assignments = new List<IOperation>();
            var assertions = new List<IInvocationOperation>();
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
            return (assignments.ToArray(), assertions.ToArray());
        }

        private static void CheckAndAddInvocation(IOperation op, ref HashSet<string> calledMethods, ref List<IInvocationOperation> invocations)
        {
            if (op.Kind == OperationKind.Invocation)
            {
                var invocationArg = (IInvocationOperation)op;
                calledMethods.Add(invocationArg.TargetMethod.Name);
                invocations.Add(invocationArg);
            }
        }

        private static void CheckAndAddFromAssignment(IOperation operation, ILocalReferenceOperation referenceArg, ref HashSet<string> calledMethods, ref List<IInvocationOperation> invocations)
        {
            if (operation.Kind == OperationKind.SimpleAssignment)
            {
                var assign = (ISimpleAssignmentOperation)operation;
                var target = (ILocalReferenceOperation)assign.Target;
                if (TestUtils.SymbolEquals(target.Local, referenceArg.Local))
                {
                    foreach (var op in assign.Value.DescendantsAndSelf())
                    {
                        CheckAndAddInvocation(op, ref calledMethods, ref invocations);
                    }
                }
            }
            if (operation.Kind == OperationKind.VariableDeclarator)
            {
                var declaration = (IVariableDeclaratorOperation)operation;
                if (TestUtils.SymbolEquals(declaration.Symbol, referenceArg.Local))
                {
                    foreach (var op in declaration.Initializer.Value.DescendantsAndSelf())
                    {
                        CheckAndAddInvocation(op, ref calledMethods, ref invocations);
                    }
                }
            }
        }

        private static Action<OperationBlockAnalysisContext> AnalyzeMethodOperations(IMethodSymbol[] relevantAssertions)
        {
            return (OperationBlockAnalysisContext context) =>
            {
                

                var blockOperation = TestUtils.GetBlockOperation(context);
                if (blockOperation is null) { return; }
                (var assignments, var assertions) = GetAssertionsAndAssignments(blockOperation, relevantAssertions);

                if (assertions.Count() <= 1) { return; }

  
                var calledMethods = new HashSet<string>();
                var invocations = new List<IInvocationOperation>();

                foreach (var assert in assertions)
                {
                    var argValue = GetAssertionValueArgument(assert);
                    if (argValue is null) { continue; }

                    // If the value argument is an invocation, it gets added imediately
                    CheckAndAddInvocation(argValue, ref calledMethods, ref invocations);

                    // If it's a reference to a value, we check which invocations are involved in that value
                    if (argValue.Kind == OperationKind.LocalReference)
                    {
                        var referenceArg = (ILocalReferenceOperation)argValue;
                        foreach (IOperation operation in assignments)
                        {
                            //We check each assignment to see if they are related to the value argument
                            CheckAndAddFromAssignment(operation, referenceArg, ref calledMethods, ref invocations);
                        }
                    }
                }

                if (calledMethods.Count > 1)
                {
                    var firstLocation = assertions.First().Syntax.GetLocation();
                    var testLocation = context.OwningSymbol.Locations.First();

                    var secondaryLocations = new List<Location>(from o in assertions select o.Syntax.GetLocation());
                    secondaryLocations.Insert(0, testLocation);


                    var diagnostic = Diagnostic.Create(Rule, firstLocation, secondaryLocations, context.OwningSymbol.Name);
                    context.ReportDiagnostic(diagnostic);
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
