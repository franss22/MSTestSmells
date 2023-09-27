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
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using EnvDTE80;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace TestSmells.AssertionRoulette
{

    public class CustomOperation : CodeActionOperation
    {
        private readonly Func<CancellationToken, Document, Task> SelectText;
        private readonly Document ChangedDocument;

        public CustomOperation(Func<CancellationToken, Document, Task> action, Document changedDocument)
        {
            this.SelectText = action;
            ChangedDocument = changedDocument;
        }

        public override void Apply(Workspace workspace, CancellationToken cancellationToken)
        {
            // Execute the custom action
            SelectText(cancellationToken, ChangedDocument).RunSynchronously();
        }
    }


    public class CustomCodeAction : CodeAction
    {
        private readonly Func<CancellationToken, Task<Document>> CreateChangedDocument;
        public override string EquivalenceKey { get; }
        public override string Title { get; }
        private Func<CancellationToken, Document, Task> SelectText;



        public CustomCodeAction(string title, Func<CancellationToken, Task<Document>> createChangedDocument, string equivalenceKey, Func<CancellationToken, Document, Task> selectText)
        {
            Title = title;
            CreateChangedDocument = createChangedDocument;
            EquivalenceKey = equivalenceKey;
            SelectText = selectText;
        }


        protected override async Task<IEnumerable<CodeActionOperation>> ComputePreviewOperationsAsync(CancellationToken cancellationToken)
        {
            var changedDocument = await CreateChangedDocument(cancellationToken).ConfigureAwait(false);
            if (changedDocument == null)
                return null;

            return new CodeActionOperation[] { new ApplyChangesOperation(changedDocument.Project.Solution), new CustomOperation(SelectText, changedDocument) };
        }

        protected override async Task<IEnumerable<CodeActionOperation>> ComputeOperationsAsync(CancellationToken cancellationToken)
        {
            var changedDocument = await CreateChangedDocument(cancellationToken);
            if (changedDocument == null)
                return null;
            var root = changedDocument.GetSyntaxRootAsync(cancellationToken);

            await SelectText(cancellationToken, changedDocument);


            return new CodeActionOperation[] { new ApplyChangesOperation(changedDocument.Project.Solution) };
        }





    }


    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AssertionRouletteCodeFixProvider)), Shared]
    public class AssertionRouletteCodeFixProvider : CodeFixProvider
    {

        [Import]
        internal SVsServiceProvider ServiceProvider { get; set; }
        public static readonly DTE2 DTE;
        public static readonly IVsTextManager TextManager;

        static AssertionRouletteCodeFixProvider()
        {
            try
            {
                DTE = Package.GetGlobalService(typeof(DTE)) as DTE2;

            }
            catch (System.Exception)
            {
                DTE = null;
            }
        }

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
                new CustomCodeAction(
                    title: CodeFixResources.CodeFixTitle,
                    createChangedDocument: (c) => AddMessageParameterAsync(context.Document, methodCall, c),
                    selectText: (c, doc) => SelectText(doc, c),
                    
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                diagnostic);
        }


        private async Task SelectText(Document document, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var changedMessage = root.DescendantNodesAndSelf()
            .Where(node => node.GetAnnotations("MessageArgument").Any()).FirstOrDefault();

            Location location = changedMessage.GetLocation();

            // Get the line and column information
            LinePosition startLinePosition = location.GetLineSpan().StartLinePosition;
            LinePosition endLinePosition = location.GetLineSpan().EndLinePosition;

            int startLine = startLinePosition.Line + 1; // Lines are zero-based, so we add 1 to get the 1-based line number
            int startColumn = startLinePosition.Character + 1 + 1; // Characters are zero-based, so we add 1 to get the 1-based column number

            int endLine = endLinePosition.Line + 1;
            int endColumn = endLinePosition.Character + 1 - 1;

            if (DTE != null)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
                EnvDTE.TextDocument textDocument = DTE.ActiveDocument.Object("TextDocument") as EnvDTE.TextDocument;
                TextSelection textSelection = textDocument.Selection as TextSelection;

                textSelection.MoveToLineAndOffset(startLine, startColumn, false);
                textSelection.MoveToLineAndOffset(endLine, endColumn, true);
            }
        }

        private async Task<Document> AddMessageParameterAsync(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            var message = Argument(
                    LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                Literal("message"))).WithAdditionalAnnotations(new SyntaxAnnotation("MessageArgument"));

            InvocationExpressionSyntax newInvocation = invocation.WithArgumentList(invocation.ArgumentList.AddArguments(message));

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newDocument = document.WithSyntaxRoot(root.ReplaceNode(invocation, newInvocation));

            return newDocument;

        }
    }
}
