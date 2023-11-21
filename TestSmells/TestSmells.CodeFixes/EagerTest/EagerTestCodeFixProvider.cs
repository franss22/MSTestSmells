using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestSmells.Compendium.EagerTest;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


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
            var diagnostics = new List<InvocationExpressionSyntax>(from l in diagnostic.AdditionalLocations.Skip(1) select (InvocationExpressionSyntax) root.FindNode(l.SourceSpan));


            //check if any of the invocations has an argument of a type not supported by the fix.
            foreach (var invocation in diagnostics)
            {
                if (invocation.ArgumentList.Arguments.Any(NotPermittedArgument))
                {
                    return;
                }
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.CodeFixTitle,
                    createChangedDocument: c => SeparateTestMethod(context.Document, testMethod, diagnostics, c),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                diagnostic);
        }

        private async Task<Document> SeparateTestMethod(Document document, MethodDeclarationSyntax methodSyntax, List<InvocationExpressionSyntax> assertionSyntaxes, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            string methodName = methodSyntax.Identifier.Text;
            var AssertReplacements = new List<List<ExpressionSyntax>>();
            foreach (var invocationExpression in assertionSyntaxes)
            {
                var relevantArguments = invocationExpression.ArgumentList.Arguments.Where(IsNotIgnorableArgument);
                var replacements = new List<ExpressionSyntax>(from arg in relevantArguments select arg.Expression);
                AssertReplacements.Add(replacements);
            }

            var assertionsWithReplacement = assertionSyntaxes.Zip(AssertReplacements, (assert, replaces) => (assert, replaces));

            var newMethods = new List<MethodDeclarationSyntax>();
            var i = 1;
            foreach (var keptAssert in assertionSyntaxes)
            {
                var newMethod = methodSyntax;
                var body = newMethod.Body;
                foreach ((var assert, var replaces) in assertionsWithReplacement.Where(a => a.assert != keptAssert))
                {
                    
                    if (replaces.Count == 0)
                    {
                        body = body.RemoveNode(assert.Parent, SyntaxRemoveOptions.KeepExteriorTrivia);
                    }
                    else if (replaces.Count == 1)
                    {
                        var leading = assert.Parent.GetLeadingTrivia();
                        var trailing = assert.Parent.GetTrailingTrivia();
                        body = body.ReplaceNode(assert.Parent, ExpressionStatement(replaces.First()).WithTrailingTrivia(trailing).WithLeadingTrivia(leading));
                    }
                    else
                    {
                        var leading = assert.Parent.GetLeadingTrivia();
                        var trailing = assert.Parent.GetTrailingTrivia();
                        
                        

                        var statements = new List<ExpressionStatementSyntax>(from rExpression in replaces select ExpressionStatement(rExpression));

                        statements[0] = statements.First().WithLeadingTrivia(leading);
                        statements[statements.Count-1] = statements.Last().WithTrailingTrivia(trailing);

                        body = body.ReplaceNode(assert.Parent, statements);

                    }
                }
                var mLeading = newMethod.GetLeadingTrivia();
                if (i != 1) { mLeading = mLeading.Insert(0, CarriageReturnLineFeed); }
                newMethod = newMethod.WithBody(body).WithIdentifier(Identifier(methodName + $"_{i}")).WithAdditionalAnnotations(Formatter.Annotation).WithLeadingTrivia(mLeading);
                i++;

                newMethods.Add(newMethod);
            }
            var newroot = root.ReplaceNode(methodSyntax, newMethods).WithAdditionalAnnotations(Formatter.Annotation);
            
            return document.WithSyntaxRoot(newroot);

        }


        private bool NotPermittedArgument(ArgumentSyntax arg)
        {
            var permittedKinds = new List<SyntaxKind> 
            { 
                 , 
            };

            return !permittedKinds.Contains(arg.Expression.Kind());
        }

        private bool IsNotIgnorableArgument(ArgumentSyntax arg)
        {
            var notIgnoredKinds = new List<SyntaxKind>
            {
                SyntaxKind.InvocationExpression,
            };
            return notIgnoredKinds.Contains(arg.Expression.Kind());
        }




    }
}
