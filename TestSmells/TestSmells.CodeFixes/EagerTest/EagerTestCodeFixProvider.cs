using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestSmells.EagerTest
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EagerTestCodeFixProvider)), Shared]
    public class EagerTestCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(EagerTestAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var testMethodSpan = diagnostic.AdditionalLocations.First().SourceSpan;
            var testMethod = root.FindToken(testMethodSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
            var diagnosticSpans = new List<InvocationExpressionSyntax>(from l in diagnostic.AdditionalLocations.Skip(1) select (InvocationExpressionSyntax) root.FindNode(l.SourceSpan));


            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.CodeFixTitle,
                    createChangedDocument: c => SeparateTestMethod(context.Document, testMethod, diagnosticSpans, c),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                diagnostic);
        }

        private async Task<Document> SeparateTestMethod(Document document, MethodDeclarationSyntax methodSpan, List<InvocationExpressionSyntax> assertionSpans, CancellationToken cancellationToken)
        {

            throw new NotImplementedException();
        }
    }
}
