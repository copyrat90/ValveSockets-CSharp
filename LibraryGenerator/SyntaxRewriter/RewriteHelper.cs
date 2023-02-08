using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace LibraryGenerator.SyntaxRewriter
{
    static class RewriteHelper
    {
        public static SyntaxToken GetRefKeyword => SyntaxFactory.Token(SyntaxTriviaList.Empty, SyntaxKind.RefKeyword,
            SyntaxFactory.TriviaList(SyntaxFactory.Space));

        public static bool TryGetQualifiedName(TypeDeclarationSyntax node, out QualifiedNameSyntax qualifiedName)
        {
            NameSyntax namespaceName;

            qualifiedName = null;

            if (node.Parent.IsKind(SyntaxKind.NamespaceDeclaration) &&
                node.Parent is NamespaceDeclarationSyntax namespaceNode)
            {
                namespaceName = namespaceNode.Name.WithoutTrivia();
            }
            else if (node.Parent!.Parent.IsKind(SyntaxKind.NamespaceDeclaration) &&
                     node.Parent.Parent is NamespaceDeclarationSyntax namespaceParent)
            {
                namespaceName = namespaceParent.Name.WithoutTrivia();
            }
            else
            {
                return false;
            }

            qualifiedName = SyntaxFactory.QualifiedName(namespaceName, SyntaxFactory.IdentifierName(node.Identifier.Text));

            return true;
        }

        public static RefTypeSyntax PointerToRefType(PointerTypeSyntax node)
        {
            if (node.ElementType.IsKind(SyntaxKind.PointerType) && node.ElementType is PointerTypeSyntax pointerElement)
            {
                return SyntaxFactory.RefType(GetRefKeyword, SyntaxFactory.ParseTypeName($"{pointerElement.ElementType.ToString()}[]")).WithTriviaFrom(node);
            }

            return SyntaxFactory.RefType(GetRefKeyword, node.ElementType.WithTriviaFrom(node));
        }

        public static ParameterListSyntax RewriteParameterList(this ParameterListSyntax node,
            Func<ParameterSyntax, bool> shouldRewriteFunction, Func<ParameterSyntax, ParameterSyntax> rewriteFunction)
        {
            bool changedParameter = false;
            var parameterList = SyntaxFactory.SeparatedList<ParameterSyntax>();

            foreach (ParameterSyntax parameter in node.Parameters)
            {
                if (shouldRewriteFunction(parameter))
                {
                    parameterList = parameterList.Add(rewriteFunction(parameter));
                    changedParameter = true;

                    continue;
                }

                parameterList = parameterList.Add(parameter);
            }

            return changedParameter ? node.WithParameters(parameterList) : node;
        }
    }
}