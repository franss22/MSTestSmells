using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Concurrent;
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


        public class FileSymbols
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

        internal static void RegisterTwoPartAnalysis(SymbolStartAnalysisContext context, FileSymbols fileSymbols)
        {
            ConcurrentBag<IInvocationOperation> writeBag = new ConcurrentBag<IInvocationOperation>();
            ConcurrentBag<IInvocationOperation> readBag = new ConcurrentBag<IInvocationOperation>();
            ConcurrentBag<ILiteralOperation> ignoredFilesBag = new ConcurrentBag<ILiteralOperation>();


            

            context.RegisterOperationAction(AnalyzeLiteralOperations(ignoredFilesBag), OperationKind.Literal);
            context.RegisterOperationAction(AnalyzeInvocationOperations(writeBag, readBag, fileSymbols), OperationKind.Invocation);


            context.RegisterSymbolEndAction(AnalyzeBags(ignoredFilesBag, writeBag, readBag));
        }

        private static Action<SymbolAnalysisContext> AnalyzeBags(ConcurrentBag<ILiteralOperation> ignoredFilesBag, ConcurrentBag<IInvocationOperation> writeBag, ConcurrentBag<IInvocationOperation> readBag)
        {
            return (SymbolAnalysisContext context) =>
            {
                if (writeBag.Count > 0 || ignoredFilesBag.Count > 0) { return; }
                foreach (var readOperation in readBag)
                {
                    var diagnostic = Diagnostic.Create(Rule, readOperation.Syntax.GetLocation(), properties: TestUtils.MethodNameProperty(context), context.Symbol.Name, readOperation.TargetMethod.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            };
        }

        private static Action<OperationAnalysisContext> AnalyzeLiteralOperations(ConcurrentBag<ILiteralOperation> ignoredFilesBag)
        {
            return (OperationAnalysisContext context) =>
            {
                var fileOptions = context.Options.AnalyzerConfigOptionsProvider.GetOptions(context.FilterTree);
                List<string> ignoredFilesList = GetIgnoredFilesFromOptions(fileOptions);

                var literal = (ILiteralOperation)context.Operation;

                if (literal.Type?.SpecialType == SpecialType.System_String && literal.ConstantValue.HasValue)
                {
                    if (ignoredFilesList.Any(f => literal.ConstantValue.Value.ToString().Contains(f)))
                    {
                        ignoredFilesBag.Add(literal);
                    }
                }
            };
        }

        private static Action<OperationAnalysisContext> AnalyzeInvocationOperations(ConcurrentBag<IInvocationOperation> writeBag, ConcurrentBag<IInvocationOperation> readBag, FileSymbols fileSymbols)
        {
            return (OperationAnalysisContext context) =>
            {
                IMethodSymbol[] readMethods = fileSymbols.ReadMethods;
                IMethodSymbol[] writeMethods = fileSymbols.WriteMethods;
                INamedTypeSymbol fileClass = fileSymbols.FileClass;
                INamedTypeSymbol fileStreamClass = fileSymbols.FileStreamClass;

                var invocation = (IInvocationOperation)context.Operation;

                //Check if the method are called from the file classes instead of any parent class
                //FileStream.ReadAsync and Stream.ReadAsync are the smae symbol,
                //but if ReadAsync is called from a FileStream object, it is a file read method
                var methodIsStatic = invocation.Instance is null;
                if (!methodIsStatic)
                {
                    var methodInstanceIsFile = TestUtils.SymbolEquals(invocation.Instance.Type, fileClass);
                    var methodInstanceIsFileStream = TestUtils.SymbolEquals(invocation.Instance.Type, fileStreamClass);
                    if (!(methodInstanceIsFile || methodInstanceIsFileStream)) { return; }
                }


                if (TestUtils.MethodIsInList(invocation.TargetMethod.OriginalDefinition, readMethods))
                {
                    readBag.Add(invocation);
                }
                if (TestUtils.MethodIsInList(invocation.TargetMethod.OriginalDefinition, writeMethods))
                {
                    writeBag.Add(invocation);
                }
            };
        }

        private static List<string> GetIgnoredFilesFromOptions(AnalyzerConfigOptions fileOptions)
        {
            var ignoredFiles = SettingSingleton.GetSettings(fileOptions, "dotnet_diagnostic.MysteryGuest.IgnoredFiles");


            var ignoredFilesList = new List<string>();
            if (ignoredFiles != null)
            {
                foreach (var filename in ignoredFiles.Split(','))
                {
                    ignoredFilesList.Add(filename.Trim());
                }
            }

            return ignoredFilesList;
        }


    }
}
