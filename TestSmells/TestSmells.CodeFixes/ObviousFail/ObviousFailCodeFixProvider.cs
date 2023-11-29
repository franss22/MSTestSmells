using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestSmells.Compendium.ObviousFail;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TestSmells.ObviousFail
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ObviousFailCodeFixProvider)), Shared]
    public class ObviousFailCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(ObviousFailAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: ObviousFail the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            //var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);
            //var AssertClass = semanticModel.Compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.Assert");
            //if (AssertClass != null) return;
            //var failMethod = AssertClass.GetMembers("Fail").FirstOrDefault();
            //if (failMethod != null) return;

            // Find the type declaration identified by the diagnostic.
            var invocation = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.CodeFixTitle,
                    createChangedDocument: c => ReplaceWithFail(context.Document, invocation, root, c),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                diagnostic);
        }

        private async Task<Document> ReplaceWithFail(Document document, InvocationExpressionSyntax assertion, SyntaxNode root, CancellationToken cancellationToken)
        {


            var newArguments = ArgumentList(SeparatedList(assertion.ArgumentList.Arguments.Skip(1)));//skip the bool argument but leave any comments
            
            var failCall = InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("Assert"),
                        IdentifierName("Fail"))).WithArgumentList(newArguments)
                        .WithAdditionalAnnotations(Simplifier.AddImportsAnnotation, Formatter.Annotation);
            
            return document.WithSyntaxRoot(root.ReplaceNode(assertion, failCall));
        }
    }
}
