using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;


namespace TestSmells.Compendium.UnknownTest
{
    public class UnknownTestAnalyzer
    {
        public const string DiagnosticId = "UnknownTest";


        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Test Smells";

        internal static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);


        internal static void RegisterTwoPartAnalysis(SymbolStartAnalysisContext context, IEnumerable<IMethodSymbol> assertionSymbols)
        {
            ConcurrentBag<IInvocationOperation> assertionBag = new ConcurrentBag<IInvocationOperation>();

            context.RegisterOperationAction(CollectAssertionsAndHelperAssertions(assertionSymbols, assertionBag), OperationKind.Invocation);
            context.RegisterSymbolEndAction(AnalyzeAssertions(assertionBag));
        }

        private static Action<SymbolAnalysisContext> AnalyzeAssertions(ConcurrentBag<IInvocationOperation> methodBag)
        {
            return (SymbolAnalysisContext context) =>
            {
                if (methodBag.Count != 0) { return; }

                var diagnostic = Diagnostic.Create(Rule, context.Symbol.Locations[0], properties: TestUtils.MethodNameProperty(context), context.Symbol.Name);
                context.ReportDiagnostic(diagnostic);
            };
        }

        private static Action<OperationAnalysisContext> CollectAssertionsAndHelperAssertions(IEnumerable<IMethodSymbol> assertionSymbols, ConcurrentBag<IInvocationOperation> methodBag)
        {
            return (OperationAnalysisContext context) =>
            {
                var fileOptions = context.Options.AnalyzerConfigOptionsProvider.GetOptions(context.FilterTree);

                var customAssertionNames = GetCustomAssertionsFromOptions(fileOptions);


                var invocation = (IInvocationOperation)context.Operation;

                var calledMethod = invocation.TargetMethod;
                if (IsCountedAsAssertion(assertionSymbols, calledMethod, customAssertionNames))
                {
                    methodBag.Add(invocation);
                }
            };
        }

        private static List<string> GetCustomAssertionsFromOptions(AnalyzerConfigOptions fileOptions)
        {
            var customAssertionNames = SettingSingleton.GetSettings(fileOptions, "dotnet_diagnostic.UnknownTest.CustomAssertions");


            var CustomAssertions = new List<string>();
            if (customAssertionNames != null)
            {
                foreach (var filename in customAssertionNames.Split(','))
                {
                    CustomAssertions.Add(filename.Trim());
                }
            }

            return CustomAssertions;
        }

        private static bool IsCountedAsAssertion(IEnumerable<IMethodSymbol> assertionSymbols, IMethodSymbol calledMethod, List<string> customAssertionNames)
        {
            return TestUtils.MethodIsInList(calledMethod, assertionSymbols) || calledMethod.Name.ToLower().Contains("assert") || customAssertionNames.Contains(calledMethod.Name);
        }
    }
}
