using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace TestSmells.Compendium.MysteryGuest
{
    public class MysteryGuestAnalyzer
    {
        public const string DiagnosticId = "MysteryGuest";


        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Test Smells";

        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);


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


        public readonly struct FileSymbols
        {
            public readonly IMethodSymbol[] WriteMethods;
            public readonly IMethodSymbol[] ReadMethods;

            public readonly INamedTypeSymbol FileClass;
            public readonly INamedTypeSymbol FileStreamClass;

            public FileSymbols(Compilation compilation)
            {
                var fileClass = compilation.GetTypeByMetadataName("System.IO.File");
                var fileReadMethods = GetMethods(fileClass, FileMethods);

                var fileStreamClass = compilation.GetTypeByMetadataName("System.IO.FileStream");
                var fileStreamReadMethods = GetMethods(fileStreamClass, FileStreamMethods);

                var readMethods = fileReadMethods.Concat(fileStreamReadMethods).ToArray();

                var fileWriteMethods = GetMethods(fileClass, FileMethodsWrite);
                var fileStreamWriteMethods = GetMethods(fileStreamClass, FileStreamMethodsWrite);
                var writeMethods = fileWriteMethods.Concat(fileStreamWriteMethods).ToArray();


                WriteMethods = writeMethods;
                ReadMethods = readMethods;
                FileClass = fileClass;
                FileStreamClass = fileStreamClass;
            }

            public bool Error()
            {
                return FileClass is null || FileStreamClass is null || WriteMethods.Length == 0 || ReadMethods.Length == 0;
            }
        }


        private static IMethodSymbol[] GetMethods(INamedTypeSymbol fileClass, string[] methodNames)
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


        internal static Action<OperationBlockAnalysisContext> AnalyzeMethodOperations(
            FileSymbols fileSymbols)
        {
            if (fileSymbols.Error()) return c => { };


            IMethodSymbol[] readMethods = fileSymbols.ReadMethods;
            IMethodSymbol[] writeMethods = fileSymbols.WriteMethods;
            INamedTypeSymbol fileClass = fileSymbols.FileClass;
            INamedTypeSymbol fileStreamClass = fileSymbols.FileStreamClass;

            return (context) =>
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


                var readOperations = new List<IInvocationOperation>();
                var writeOperations = new List<IInvocationOperation>();
                foreach (var operation in blockOperation.Descendants())
                {
                    if (operation is ILiteralOperation literal)
                    {
                        if (literal.Type != null && literal.Type.SpecialType == SpecialType.System_String && literal.ConstantValue.HasValue)
                        {
                            if (ignoredFilesList.Any(f => literal.ConstantValue.Value.ToString().Contains(f)))
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
                        var methodInstanceIsFile = TestUtils.SymbolEquals(invocationOperation.Instance.Type, fileClass);
                        var methodInstanceIsFileStream = TestUtils.SymbolEquals(invocationOperation.Instance.Type, fileStreamClass);
                        if (!(methodInstanceIsFile || methodInstanceIsFileStream)) { continue; }

                    }


                    if (MethodIsInList(invocationOperation.TargetMethod.OriginalDefinition, readMethods))
                    {
                        readOperations.Add(invocationOperation);
                    }
                    if (MethodIsInList(invocationOperation.TargetMethod.OriginalDefinition, writeMethods))
                    {
                        writeOperations.Add(invocationOperation);
                    }
                }

                if (writeOperations.Count > 0) { return; }
                foreach (var readOperation in readOperations)
                {
                    var args = readOperation.Arguments;
                    var isIgnored = false;
                    foreach (var argument in args)
                    {
                        var val = argument.Value;
                    }
                    if (isIgnored)
                    {
                        continue;
                    }
                    var diagnostic = Diagnostic.Create(Rule, readOperation.Syntax.GetLocation(), properties: TestUtils.MethodNameProperty(context), context.OwningSymbol.Name, readOperation.TargetMethod.Name);
                    context.ReportDiagnostic(diagnostic);
                }


            };
        }

        private static bool MethodIsInList(IMethodSymbol symbol, ISymbol[] relevantAssertions)
        {
            if (symbol == null) return false;

            foreach (var function in relevantAssertions)
            {
                if (TestUtils.SymbolEquals(symbol.OriginalDefinition, function))
                {
                    return true;

                }
            }
            return false;

        }
    }
}
