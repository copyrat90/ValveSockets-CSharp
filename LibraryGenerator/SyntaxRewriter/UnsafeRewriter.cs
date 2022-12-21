using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LibraryGenerator.SyntaxRewriter;

public class UnsafeRewriter : CSharpSyntaxRewriter
{
    private ParameterListSyntax VisitMethodParameterList(ParameterListSyntax node)
    {
        bool changedParameter = false;
        var parameterList = SyntaxFactory.SeparatedList<ParameterSyntax>();

        foreach (ParameterSyntax parameter in node.Parameters)
        {
            if (parameter.Type.IsKind(SyntaxKind.PointerType))
            {
                if (parameter.Type.ToString() == "void*")
                {
                    // TODO: Deal with NativeTypeName attributes
                    if (!parameter.AttributeLists.Any())
                    {
                        continue;
                    }
                }

                var newParameter = parameter.WithType(SyntaxFactory.RefType(
                        SyntaxFactory.Token(SyntaxTriviaList.Empty, SyntaxKind.RefKeyword, SyntaxFactory.TriviaList(SyntaxFactory.Space)),
                        SyntaxFactory.ParseTypeName(parameter.Type.ToString()[..^1]).WithTriviaFrom(parameter.Type))
                );

                parameterList.Add(newParameter);
                changedParameter = true;

                continue;
            }

            parameterList.Add(parameter);
        }

        return changedParameter ? node.WithParameters(parameterList) : node;
    }

    private MemberDeclarationSyntax VisitClassMember(MemberDeclarationSyntax node)
    {
        if (node.IsKind(SyntaxKind.MethodDeclaration))
        {
            var newNode = node;

            foreach (var childNode in node.ChildNodes())
            {
                if (childNode.IsKind(SyntaxKind.ParameterList) && childNode is ParameterListSyntax parameterList)
                {
                    var newParameterList = VisitMethodParameterList(parameterList);

                    if (parameterList != newParameterList)
                    {
                        newNode = newNode.ReplaceNode(parameterList, newParameterList);
                    }
                }
            }

            return newNode;
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