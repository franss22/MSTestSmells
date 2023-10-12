using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestSmells.RedundantAssertion
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RedundantAssertionCodeFixProvider)), Shared]
    public class RedundantAssertionCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(RedundantAssertionAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var assertion = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().First();
            if (assertion.Parent.IsKind(SyntaxKind.ExpressionStatement))
            {
                context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.CodeFixTitle,
                    createChangedDocument: c => DeleteAssertionAsync(context.Document, root, (ExpressionStatementSyntax)assertion.Parent, c),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                diagnostic);
            }
            
        }

        private async Task<Document> DeleteAssertionAsync(Document document, SyntaxNode root, ExpressionStatementSyntax assertionStatement, CancellationToken cancellationToken)
        {
            return document.WithSyntaxRoot(root.RemoveNode(assertionStatement, SyntaxRemoveOptions.KeepNoTrivia));
        }
    }
}
