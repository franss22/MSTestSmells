using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace TestSmells.Compendium.EagerTest
{
    public class EagerTestAnalyzer
    {
        public const string DiagnosticId = "EagerTest";


        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Test Smells";

        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);


        private static readonly string[] ParamNames =
        {
            "actual",
            "value",
            "condition",
            "collection",
        };



        private static (IOperation[], IInvocationOperation[]) GetAssertionsAndAssignments(IBlockOperation blockOperation, IMethodSymbol[] relevantAssertions)
        {
            var assignments = new List<IOperation>();
            var assertions = new List<IInvocationOperation>();
            foreach (var operation in blockOperation.Descendants())
            {
                if (operation.Kind == OperationKind.SimpleAssignment || operation.Kind == OperationKind.VariableDeclarator)
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

        internal static INamespaceSymbol GetSystemNamespace(Compilation compilation)
        {
            var system = compilation.GetTypeByMetadataName("System.Exception").ContainingNamespace;
            return system;
        }

        internal static Action<OperationBlockAnalysisContext> AnalyzeMethodBody(IMethodSymbol[] relevantAssertions, INamespaceSymbol systemNamespace)
        {
            return (context) =>
            {


                var blockOperation = TestUtils.GetBlockOperation(context);
                if (blockOperation is null) { return; }
                (var assignments, var assertions) = GetAssertionsAndAssignments(blockOperation, relevantAssertions);

                if (assertions.Count() <= 1) { return; }


                var calledMethods = new HashSet<ISymbol>(SymbolEqualityComparer.Default);
                var invocations = new List<IInvocationOperation>();

                foreach (var assert in assertions)
                {
                    var argValue = GetAssertionValueArgument(assert);
                    if (argValue is null) { continue; }
                    foreach (var operation in argValue.DescendantsAndSelf())
                    {
                        // If the value argument is an invocation, it gets added imediately
                        CheckAndAddInvocation(operation, ref calledMethods, ref invocations);

                        // If it's a reference to a value, we check which invocations are involved in that value
                        if (operation.Kind == OperationKind.LocalReference)
                        {
                            //throw new Exception("TestException");
                            var referenceArg = (ILocalReferenceOperation)operation;
                            foreach (IOperation assignment in assignments)
                            {
                                //We check each assignment to see if they are related to the value argument
                                CheckAndAddFromAssignment(assignment, referenceArg, ref calledMethods, ref invocations);
                            }
                        }
                    }
                                       
                }

                var calledMethodsCopy = calledMethods.ToArray();
                foreach (var method in calledMethodsCopy)
                {
                    var methodNamespace = method.ContainingNamespace;
                    
                    while (methodNamespace != null)
                    {
                        if (TestUtils.SymbolEquals(methodNamespace, systemNamespace))
                        {
                            calledMethods.Remove(method);
                            break;
                        }
                        methodNamespace = methodNamespace.ContainingNamespace;
                    }
                }

                if (calledMethods.Count > 1)
                {
                    var firstLocation = assertions.First().Syntax.GetLocation();
                    var testLocation = context.OwningSymbol.Locations.First();

                    var secondaryLocations = new List<Location>(from o in assertions select o.Syntax.GetLocation());
                    secondaryLocations.Insert(0, testLocation);

                    var methodNames = string.Join(", ", calledMethods.Select(m => m.Name));


                    var diagnostic = Diagnostic.Create(Rule, testLocation, secondaryLocations, properties: TestUtils.MethodNameProperty(context), context.OwningSymbol.Name, methodNames);
                    context.ReportDiagnostic(diagnostic);
                }
            };


        }


        private static void CheckAndAddInvocation(IOperation op, ref HashSet<ISymbol> calledMethods, ref List<IInvocationOperation> invocations)
        {
            if (op.Kind == OperationKind.Invocation)
            {
                var invocation = (IInvocationOperation)op;
                calledMethods.Add(invocation.TargetMethod);
                invocations.Add(invocation);
            }
        }

        private static void CheckAndAddFromAssignment(IOperation operation, ILocalReferenceOperation referenceArg, ref HashSet<ISymbol> calledMethods, ref List<IInvocationOperation> invocations)
        {
            if (operation.Kind == OperationKind.SimpleAssignment)
            {
                var assign = (ISimpleAssignmentOperation)operation;
                ILocalReferenceOperation target;
                if (assign.Target.Kind == OperationKind.LocalReference)
                {
                    target = (ILocalReferenceOperation)assign.Target;
                }
                else return;


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
                if (declaration.Initializer is null) return;

                if (TestUtils.SymbolEquals(declaration.Symbol, referenceArg.Local))
                {
                    foreach (var op in declaration.Initializer.Value.DescendantsAndSelf())
                    {
                        CheckAndAddInvocation(op, ref calledMethods, ref invocations);
                    }
                }
            }
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
