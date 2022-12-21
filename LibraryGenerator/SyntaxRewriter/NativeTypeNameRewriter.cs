using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace LibraryGenerator.SyntaxRewriter;

public class NativeTypeNameRewriter : CSharpSyntaxRewriter, ISyntaxRewriter
{
    private SyntaxNode RewriteMethodReturnType(MethodDeclarationSyntax node)
    {
        return node;
    }

    private SyntaxNode RewriteParameterType(ParameterSyntax node)
    {
        return node;
    }

    private SyntaxNode RewriteFieldType(FieldDeclarationSyntax node)
    {
        return node;
    }

    private SyntaxNode RewriteEnumType(EnumDeclarationSyntax node)
    {
        return node;
    }

    private SyntaxNode RewritePropertyType(PropertyDeclarationSyntax node)
    {
        return node;
    }

    public override SyntaxNode VisitAttribute(AttributeSyntax node)
    {
        if (node.Name.ToString() == "NativeTypeName")
        {
            var parent = (node.Parent as AttributeListSyntax)!;
            var targetNode = parent.Parent!;

            if (parent.Target?.Identifier.ToString() == "return")
            {
                return node.ReplaceNode(targetNode, RewriteMethodReturnType(targetNode as MethodDeclarationSyntax));
            }

            switch (targetNode.Kind())
            {
                case SyntaxKind.Parameter:
                    return node.ReplaceNode(targetNode, RewriteParameterType(targetNode as ParameterSyntax));
                case SyntaxKind.FieldDeclaration:
                    return node.ReplaceNode(targetNode, RewriteFieldType(targetNode as FieldDeclarationSyntax));
                case SyntaxKind.EnumDeclaration:
                    return node.ReplaceNode(targetNode, RewriteEnumType(targetNode as EnumDeclarationSyntax));
                case SyntaxKind.PropertyDeclaration:
                    return node.ReplaceNode(targetNode, RewritePropertyType(targetNode as PropertyDeclarationSyntax));
                default:
                    Console.WriteLine($"\tNo conversion found for target node kind: {targetNode.Kind()}");
                    break;
            }
        }

        return node;
    }

    public SyntaxNode FixupVisit(SyntaxNode node)
    {
        return node;
    }
}