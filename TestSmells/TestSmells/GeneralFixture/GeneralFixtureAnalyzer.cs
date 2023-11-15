using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace TestSmells.GeneralFixture
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class GeneralFixtureAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "GeneralFixture";


        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Test Smells";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }


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
            var initTestMethodAttr = context.Compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute");
            if (initTestMethodAttr is null) { return; }
            var testMethodAttr = context.Compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute");
            if (testMethodAttr is null) { return; }




            var analyzeClass = AnalyzeClassSymbol(testClassAttr, initTestMethodAttr, testMethodAttr);

            context.RegisterSymbolStartAction(analyzeClass, SymbolKind.NamedType);
        }

        private static Action<SymbolStartAnalysisContext> AnalyzeClassSymbol(INamedTypeSymbol testClassAttr, INamedTypeSymbol initTestMethodAttr, INamedTypeSymbol testMethodAttr)
        {
            return (SymbolStartAnalysisContext context) =>
            {
                var classSymbol = (INamedTypeSymbol)context.Symbol;
                if (!TestUtils.AttributeIsInSymbol(testClassAttr, classSymbol)) { return; }

                IMethodSymbol testInitMethod = null;
                var testMethods = new List<IMethodSymbol>();
                var fields = new List<IFieldSymbol>();
                var members = classSymbol.GetMembers();
                foreach (var member in members)
                {
                    if (member.Kind == SymbolKind.Method)
                    {
                        if (TestUtils.AttributeIsInSymbol(testMethodAttr, member)) { testMethods.Add((IMethodSymbol)member); }
                        if (TestUtils.AttributeIsInSymbol(initTestMethodAttr, member)) { testInitMethod = (IMethodSymbol)member; }

                    }
                    if (member.Kind == SymbolKind.Field)
                    {
                        fields.Add((IFieldSymbol)member);
                    }
                }
                if (testInitMethod == null) return;

                var initFields = new ConcurrentBag<Tuple<IFieldSymbol, Location>>();

                var usedFieldsPerTest = new ConcurrentDictionary<String, List<IFieldSymbol>>();


                var analyzeOperations = AnalyzeMethodOperations(fields, testInitMethod, testMethods, initFields, usedFieldsPerTest);
                context.RegisterOperationAction(analyzeOperations, OperationKind.MethodBody);


                var analyzeSymbolEnd = AnalyzeSymbolEnd(testInitMethod, initFields, usedFieldsPerTest);
                context.RegisterSymbolEndAction(analyzeSymbolEnd);

            };
        }

        private static Action<SymbolAnalysisContext> AnalyzeSymbolEnd(IMethodSymbol testInitMethod,
            ConcurrentBag<Tuple<IFieldSymbol, Location>> initFields,
            ConcurrentDictionary<string, List<IFieldSymbol>> usedFieldsPerTest)
        {
            return (SymbolAnalysisContext context) =>
            {
                foreach (var pair in usedFieldsPerTest)
                {
                    var unusedFields = new List<Tuple<IFieldSymbol, Location>>(initFields);
                    var testMethodName = pair.Key;
                    var usedFields = pair.Value;
                    foreach (var usedField in usedFields)
                    {
                        foreach (var fieldTuple in initFields.Where(t => TestUtils.SymbolEquals(t.Item1, usedField)))
                        {
                            unusedFields.Remove(fieldTuple);
                        }
                    }

                    foreach (var (symbol, location) in unusedFields)
                    {


                        var diagnostic = Diagnostic.Create(Rule, location, properties: TestUtils.ImmProperty("MethodName", testInitMethod.ToString()), symbol.Name, testMethodName);
                        context.ReportDiagnostic(diagnostic);
                    }
                    
                }
            };

        }

        private static Action<OperationAnalysisContext> AnalyzeMethodOperations(List<IFieldSymbol> fields,
            IMethodSymbol testInitMethod,
            List<IMethodSymbol> testMethods,
            ConcurrentBag<Tuple<IFieldSymbol, Location>> initFields,
            ConcurrentDictionary<String, List<IFieldSymbol>> usedFieldsPerTest
            )
        {
            return (OperationAnalysisContext context) =>
            {
                var method = (IMethodBodyOperation)context.Operation;

                var methodSymbol = context.ContainingSymbol;
                //find all defined fields in the init method
                if (TestUtils.SymbolEquals(methodSymbol, testInitMethod))
                {
                    //find all fields that are assigned in the init method
                    var fieldAssignments = method.Descendants()
                            .Where(o => o.Kind == OperationKind.SimpleAssignment)
                            .Cast<ISimpleAssignmentOperation>()
                            .Where(a => a.Target.Kind == OperationKind.FieldReference);
                    foreach (var assignment in fieldAssignments)
                    {
                        var targetField = (IFieldReferenceOperation)assignment.Target;
                        var fieldSymbol = targetField.Field;
                        initFields.Add(new Tuple<IFieldSymbol, Location>(fieldSymbol, assignment.Syntax.GetLocation()));
                    }
                    return;

                }
                //find all fields used in a test method
                else if (testMethods.Any(ms => TestUtils.SymbolEquals(methodSymbol, ms)))
                {
                    var name = methodSymbol.Name;
                    var used_fields = new List<IFieldSymbol>();
                    //find all fields that are assigned in the init method
                    var fieldReferences = method.Descendants()
                            .Where(o => o.Kind == OperationKind.FieldReference)
                            .Cast<IFieldReferenceOperation>();
                    foreach (var fieldRef in fieldReferences)
                    {
                        var fieldSymbol = fieldRef.Field;
                        used_fields.Add(fieldSymbol);
                    }
                    usedFieldsPerTest.TryAdd(name, used_fields);
                    return;
                }

            };
        }


    }
}
