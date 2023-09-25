using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;



namespace TestSmells.MagicNumber

{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MagicNumberCodeFixProvider)), Shared]
    public class MagicNumberCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(MagicNumberAnalyzer.DiagnosticId); }
        }

        //public sealed override FixAllProvider GetFixAllProvider()
        //{
        //    // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
        //    return WellKnownFixAllProviders.BatchFixer;
        //}

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var methodCall = (ArgumentSyntax) root.FindNode(diagnosticSpan);

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.CodeFixTitle,
                    createChangedDocument: c => ExtractConstant(context.Document, methodCall, c),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                diagnostic);
        }

        private async Task<Document> ExtractConstant(Document document, ArgumentSyntax argument, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var semanticModel = await document.GetSemanticModelAsync();


            var argumentList = (ArgumentListSyntax)argument.Parent;
            var argumentValue = argument.Expression;
            var invocation = (InvocationExpressionSyntax)argumentList.Parent;

            var parameterName = ((IArgumentOperation)semanticModel.GetOperation(argument, cancellationToken)).Parameter.Name;



            var varname = Identifier(parameterName).WithAdditionalAnnotations(RenameAnnotation.Create());
            var typeInfo = semanticModel.GetTypeInfo(argumentValue).Type;

            var type = SyntaxGenerator.GetGenerator(document).TypeExpression(typeInfo);
            LocalDeclarationStatementSyntax localDeclaration = (LocalDeclarationStatementSyntax)SyntaxGenerator.GetGenerator(document).LocalDeclarationStatement(typeInfo, "constant_name", argumentValue, true);
            foreach (var token in localDeclaration.DescendantTokens())
            {
                if (token.Text == "constant_name")
                {
                    localDeclaration = localDeclaration.ReplaceToken(token, varname);
                }
            }

            var newInvocation = invocation.WithArgumentList(argumentList.ReplaceNode(argument, Argument(IdentifierName(varname))));
            
            var editor = new SyntaxEditor(root, document.Project.Solution.Services);
            editor.InsertBefore(invocation.Parent, new[] { localDeclaration});
            editor.ReplaceNode(invocation, newInvocation);
            
            var newdocument = document.WithSyntaxRoot(editor.GetChangedRoot());

            //var constantValue = 
            return newdocument;
        }

        private LocalDeclarationStatementSyntax MakeLocalDeclaration(SyntaxToken identifier, ExpressionSyntax argumentValue, SyntaxNode type) {



            return LocalDeclarationStatement(
                                    VariableDeclaration(PredefinedType(Token(type.Kind())
                                        ))
                                    .WithVariables(
                                        SingletonSeparatedList<VariableDeclaratorSyntax>(
                                            VariableDeclarator(identifier)
                                            .WithInitializer(
                                                EqualsValueClause(
                                                    argumentValue)
                                                .WithEqualsToken(
                                                    Token(
                                                        TriviaList(Space),
                                                        SyntaxKind.EqualsToken,
                                                        TriviaList(Space)))))))
                                .WithModifiers(
                                        TokenList(
                                            Token(SyntaxKind.ConstKeyword)))
                                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                                .WithAdditionalAnnotations(Formatter.Annotation);
        }
    }
}
