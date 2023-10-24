using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestSmells.Compendium.IgnoredTest;

namespace TestSmells.IgnoredTest
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IgnoredTestCodeFixProvider)), Shared]
    public class IgnoredTestCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(IgnoredTestAnalyzer.DiagnosticId); }
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
            var ignoreAttribute = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<AttributeSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.CodeFixTitle,
                    createChangedDocument: c => DeleteIgnoreAttributeAsync(context.Document, root, ignoreAttribute, c),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                diagnostic);
        }

        private async Task<Document> DeleteIgnoreAttributeAsync(Document document, SyntaxNode root, AttributeSyntax ignoreAttr, CancellationToken cancellationToken)
        {

            var attributeList = (AttributeListSyntax)ignoreAttr.Parent;
            if (attributeList.Attributes.Count == 1)
            {
                root = root.RemoveNode(attributeList, SyntaxRemoveOptions.KeepNoTrivia);
            }
            else
            {
                var newList = attributeList.WithAttributes(attributeList.Attributes.Remove(ignoreAttr));
                root = root.ReplaceNode(attributeList, newList);
            }


            return document.WithSyntaxRoot(root);
        }
    }
}
