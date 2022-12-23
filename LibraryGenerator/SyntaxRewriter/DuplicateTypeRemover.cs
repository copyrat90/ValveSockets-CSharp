using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace LibraryGenerator.SyntaxRewriter;

public class DuplicateTypeRemover : CSharpSyntaxRewriter, ISyntaxRewriter
{
    public bool NeedsFixupVisit => false;

    private readonly HashSet<string> typeNames = new();

    public override SyntaxNode VisitStructDeclaration(StructDeclarationSyntax node)
    {
        if (!RewriteHelper.TryGetQualifiedName(node, out QualifiedNameSyntax qualifiedName))
        {
            return node;
        }

        if (typeNames.Contains(qualifiedName.ToString()))
        {
            return node.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia);
        }

        typeNames.Add(qualifiedName.ToString());

        return node;
    }

    public SyntaxNode FixupVisit(SyntaxNode node)
    {
        throw new NotImplementedException();
    }
}