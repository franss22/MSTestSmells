using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace TestSmells.MysteryGuest
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MysteryGuestAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "MysteryGuest";


        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Test Smells";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }


        private static readonly string[] FileMethods =
        {
            "ReadAllBytes",
            "ReadAllBytesAsync",
            "ReadAllLines",
            "ReadAllLinesAsync",
            "ReadAllText",
            "ReadAllTextAsync",
            "ReadLines",
            "ReadLinesAsync",
            "OpenRead"
        };

        private static readonly string[] FileMethodsWrite = 
        {
            "AppendAllLines",
            "AppendAllLinesAsync",
            "AppendAllText",
            "AppendAllTextAsync",
            "AppendText",
            "Create",
            "CreateText",
            "OpenWrite",
            "WriteAllBytes",
            "WriteAllBytesAsync",
            "WriteAllLines",
            "WriteAllLinesAsync",
            "WriteAllText",
            "WriteAllTextAsync",
        };

        private static readonly string[] FileStreamMethods =
        {
            "BeginRead",
            "EndRead",
            "Read",
            "ReadAsync",
            "ReadByte",
        };

        private static readonly string[] FileStreamMethodsWrite = 
        {
            "BeginWrite",
            "EndWrite",
            "Write",
            "WriteAsync",
            "WriteByte",
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

            var fileClass = context.Compilation.GetTypeByMetadataName("System.IO.File");
            var fileReadMethods = GetFileReadMethods(context.Compilation, fileClass, FileMethods);

            var fileStreamClass = context.Compilation.GetTypeByMetadataName("System.IO.FileStream");
            var fileStreamReadMethods = GetFileReadMethods(context.Compilation, fileStreamClass, FileStreamMethods);

            var readMethods = fileReadMethods.Concat(fileStreamReadMethods).ToArray();
            if (readMethods.Length == 0) return;

            var fileWriteMethods = GetFileReadMethods(context.Compilation, fileClass, FileMethodsWrite);
            var fileStreamWriteMethods = GetFileReadMethods(context.Compilation, fileStreamClass, FileStreamMethodsWrite);
            var writeMethods = fileWriteMethods.Concat(fileStreamWriteMethods).ToArray();
            if (writeMethods.Length == 0) return;



            var analyzeMethod = FilterMethods(testClassAttr, testMethodAttr, readMethods, writeMethods, fileClass, fileStreamClass);



            context.RegisterSymbolStartAction(analyzeMethod, SymbolKind.Method);

        }



        private IMethodSymbol[] GetFileReadMethods(Compilation compilation, INamedTypeSymbol fileClass, string[] methodNames)
        {
            var methods = new List<IMethodSymbol>();
            while (fileClass != null)
            {
                foreach (var methodName in methodNames)
                {
                    foreach (var method in fileClass.GetMembers(methodName))
                    {
                        methods.Add((IMethodSymbol)method);
                    }
                }
                fileClass = fileClass.BaseType;
            }

            return methods.ToArray();
        }


        private Action<SymbolStartAnalysisContext> FilterMethods(
            INamedTypeSymbol testClassAttr, 
            INamedTypeSymbol testMethodAttr, 
            IMethodSymbol[] readMethods, 
            IMethodSymbol[] writeMethods,
            INamedTypeSymbol fileClass,
            INamedTypeSymbol fileStreamClass)
        {
            return (SymbolStartAnalysisContext context) =>
            {
                if (!TestUtils.TestMethodInTestClass(context, testClassAttr, testMethodAttr)) { return; }
                var operationBlockAnalisis = AnalyzeMethodOperations(readMethods, writeMethods, fileClass, fileStreamClass);
                context.RegisterOperationBlockAction(operationBlockAnalisis);
            };
        }

        private static Action<OperationBlockAnalysisContext> AnalyzeMethodOperations(
            IMethodSymbol[] fileReadMethods, 
            IMethodSymbol[] fileWriteMethods,
            INamedTypeSymbol fileClass,
            INamedTypeSymbol fileStreamClass)
        {
            return (OperationBlockAnalysisContext context) =>
            {
                var fileOptions = context.Options.AnalyzerConfigOptionsProvider.GetOptions(context.FilterTree);
                fileOptions.TryGetValue("dotnet_diagnostic.MysteryGuest.IgnoredFiles", out var ignoredFiles);
                var ignoredFilesList = new List<string>();
                if (ignoredFiles != null)
                {
                    foreach (var filename in ignoredFiles.Split(','))
                    {
                        ignoredFilesList.Add(filename.Trim());
                    }
                }
                var blockOperation = TestUtils.GetBlockOperation(context);
                if (blockOperation == null) { return; }

                var descendants = blockOperation.Descendants();

                var readOperations = new List<IInvocationOperation>();
                var writeOperations = new List<IInvocationOperation>();
                foreach (var operation in descendants)
                {
                    if (operation is ILiteralOperation literal) 
                    {
                        if (literal.Type != null && literal.Type.SpecialType == SpecialType.System_String && literal.ConstantValue.HasValue)
                        {
                            if (ContainsStringFromList(literal.ConstantValue.Value.ToString(), ignoredFilesList))
                            {
                                return;
                            }
                        }
                        
                    }
                    if (operation.Kind != OperationKind.Invocation) { continue; }
                    var invocationOperation = (IInvocationOperation)operation;
                    var methodIsStatic = invocationOperation.Instance is null;
                    if (!methodIsStatic)
                    {
                        var methodInstanceIsFile = SymbolEqualityComparer.Default.Equals(invocationOperation.Instance.Type, fileClass);
                        var methodInstanceIsFileStream = SymbolEqualityComparer.Default.Equals(invocationOperation.Instance.Type, fileStreamClass);
                        if (!(methodInstanceIsFile || methodInstanceIsFileStream)) { continue; }

                    }


                    if (MethodIsInList(invocationOperation.TargetMethod.OriginalDefinition, fileReadMethods))
                    {
                        readOperations.Add(invocationOperation);
                    }
                    if (MethodIsInList(invocationOperation.TargetMethod.OriginalDefinition, fileWriteMethods))
                    {
                        writeOperations.Add(invocationOperation);
                    }
                }

                if(writeOperations.Count > 0) { return; }
                foreach (var readOperation in readOperations)
                {
                    var args = readOperation.Arguments;
                    var isIgnored = false;
                    foreach (var argument in args)
                    {
                        var val = argument.Value;
                    }
                    if ( isIgnored)
                    {
                        continue;
                    }
                    var diagnostic = Diagnostic.Create(Rule, readOperation.Syntax.GetLocation(), context.OwningSymbol.Name, readOperation.TargetMethod.Name);
                    context.ReportDiagnostic(diagnostic);
                }
                

            };
        }

        private static bool ContainsStringFromList(string url, IEnumerable<string> stringList)
        {
            foreach (var item in stringList)
            {
                if (url.Contains(item)) return true;
            }
            return false;
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
