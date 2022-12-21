using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibraryGenerator.SyntaxRewriter;

public class UnsafeRewriter : CSharpSyntaxRewriter, ISyntaxRewriter
{
    private RefTypeSyntax RewriteMethodPointerType(PointerTypeSyntax node)
    {
        return SyntaxFactory.RefType(
            SyntaxFactory.Token(SyntaxTriviaList.Empty, SyntaxKind.RefKeyword,
                SyntaxFactory.TriviaList(SyntaxFactory.Space)),
            node.ElementType.WithTriviaFrom(node));
    }

    private ParameterListSyntax VisitMethodParameterList(ParameterListSyntax node)
    {
        bool changedParameter = false;
        var parameterList = SyntaxFactory.SeparatedList<ParameterSyntax>();

        foreach (ParameterSyntax parameter in node.Parameters)
        {
            if (parameter.Type.IsKind(SyntaxKind.PointerType) && parameter.Type is PointerTypeSyntax pointerParam)
            {
                parameterList = parameterList.Add(parameter.WithType(RewriteMethodPointerType(pointerParam)));
                changedParameter = true;

                continue;
            }

            parameterList = parameterList.Add(parameter);
        }

        return changedParameter ? node.WithParameters(parameterList) : node;
    }

    private MemberDeclarationSyntax VisitClassMember(MemberDeclarationSyntax node)
    {
        if (node.IsKind(SyntaxKind.MethodDeclaration))
        {
            foreach (var childNode in node.ChildNodes())
            {
                if (childNode.IsKind(SyntaxKind.PointerType) && childNode is PointerTypeSyntax pointerType)
                {
                    node = node.ReplaceNode(pointerType, RewriteMethodPointerType(pointerType));
                }

                if (childNode.IsKind(SyntaxKind.ParameterList) && childNode is ParameterListSyntax parameterList)
                {
                    var newParameterList = VisitMethodParameterList(parameterList);

                    if (parameterList != newParameterList)
                    {
                        node = node.ReplaceNode(parameterList, newParameterList);
                    }
                }
            }
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

    public SyntaxNode FixupVisit(SyntaxNode node)
    {
        return node;
    }
}