using EnvDTE;
using Microsoft.VisualStudio.Shell;
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
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Document = Microsoft.CodeAnalysis.Document;

namespace TestSmells.AssertionRoulette
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AssertionRouletteCodeFixProvider)), Shared]
    public class AssertionRouletteCodeFixProvider : CodeFixProvider
    {

        [Import]
        internal SVsServiceProvider ServiceProvider { get; set; }
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(AssertionRouletteAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;


            var methodCall = (InvocationExpressionSyntax)root.FindNode(diagnosticSpan);

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.CodeFixTitle,
                    createChangedDocument: c => AddMessageParameterAsync(context.Document, methodCall, c),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                diagnostic);
        }

        private async Task<Document> AddMessageParameterAsync(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            //ThreadHelper.ThrowIfNotOnUIThread();
            DTE dte = (DTE)ServiceProvider.GetService(typeof(DTE));

            var parameters = invocation.ArgumentList;
                var message = Argument(
                    LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                Literal("message")).WithAdditionalAnnotations(RenameAnnotation.Create()));

            InvocationExpressionSyntax newInvocation = invocation.WithArgumentList(parameters.AddArguments(message));

                var root = await document.GetSyntaxRootAsync(cancellationToken);
                var newDocument = document.WithSyntaxRoot(root.ReplaceNode(invocation, newInvocation));
                return newDocument;

        }
    }
}
