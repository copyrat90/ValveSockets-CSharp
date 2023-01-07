using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace LibraryGenerator.SyntaxRewriter;

public class UnsafeRewriter : CSharpSyntaxRewriter, ISyntaxRewriter
{
    public bool NeedsFixupVisit => false;

    public SyntaxNode FixupVisit(SyntaxNode node)
    {
        throw new NotImplementedException();
    }

    private static bool ShouldRewriteParameter(ParameterSyntax parameter) =>
        parameter.Type.IsKind(SyntaxKind.PointerType) && !parameter.Modifiers.Any(SyntaxKind.RefKeyword);

    private static ParameterSyntax RewriteParameter(ParameterSyntax parameter) =>
        parameter.WithType(RewriteHelper.PointerToRefType(parameter.Type as PointerTypeSyntax));

    private BlockSyntax FixReturnStatements(BlockSyntax node)
    {
        var statements = SyntaxFactory.List<StatementSyntax>();

        foreach (StatementSyntax statement in node.Statements)
        {
            if (statement.IsKind(SyntaxKind.ReturnStatement) && statement is ReturnStatementSyntax returnStatement)
            {
                statements = statements.Add(returnStatement.WithExpression(
                    SyntaxFactory.RefExpression(RewriteHelper.GetRefKeyword, returnStatement.Expression!)));

                continue;
            }

            statements = statements.Add(statement);
        }

        return node.WithStatements(statements);
    }

    private MemberDeclarationSyntax VisitClassMember(MemberDeclarationSyntax node)
    {
        if (node.IsKind(SyntaxKind.MethodDeclaration) && node is MethodDeclarationSyntax method)
        {
            var newMethod = method;
            bool changedReturnType = false;

            if (method.ReturnType.IsKind(SyntaxKind.PointerType))
            {
                newMethod = newMethod.WithReturnType(
                    RewriteHelper.PointerToRefType(newMethod.ReturnType as PointerTypeSyntax));
                changedReturnType = true;
            }

            foreach (var childNode in node.ChildNodes())
            {
                if (childNode.IsKind(SyntaxKind.ParameterList) && childNode is ParameterListSyntax parameterList)
                {
                    var newParameterList = parameterList.RewriteParameterList(ShouldRewriteParameter, RewriteParameter);

                    if (parameterList != newParameterList)
                    {
                        newMethod = newMethod.WithParameterList(newParameterList);
                    }
                }

                if (changedReturnType && childNode.IsKind(SyntaxKind.Block) && childNode is BlockSyntax block)
                {
                    newMethod = newMethod.WithBody(FixReturnStatements(block));
                }
            }

            return node.ReplaceNode(method, newMethod);
        }

        return node;
    }

    public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        SyntaxTokenList newModifiers = node.Modifiers;

        foreach (SyntaxToken modifier in node.Modifiers)
        {
            if (modifier.IsKind(SyntaxKind.UnsafeKeyword))
            {
                newModifiers = node.Modifiers.Remove(modifier);
                break;
            }
        }

        if (node.Modifiers != newModifiers)
        {
            var newMembers = SyntaxFactory.List<MemberDeclarationSyntax>();

            foreach (var member in node.Members)
            {
                newMembers = newMembers.Add(VisitClassMember(member));
            }

            return node.ReplaceNode(node, node.WithModifiers(newModifiers).WithMembers(newMembers));
        }

        return node;
    }
}