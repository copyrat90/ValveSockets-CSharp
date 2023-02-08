using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace LibraryGenerator.SyntaxRewriter
{
    public class DuplicateTypeRemover : CSharpSyntaxRewriter, ISyntaxRewriter
    {
        private readonly HashSet<string> _typeNames = new();
        public bool NeedsFixupVisit => false;

        public SyntaxNode FixupVisit(SyntaxNode node)
        {
            throw new NotImplementedException();
        }

        public override SyntaxNode VisitStructDeclaration(StructDeclarationSyntax node)
        {
            if (!RewriteHelper.TryGetQualifiedName(node, out QualifiedNameSyntax qualifiedName))
            {
                return node;
            }

            if (_typeNames.Contains(qualifiedName.ToString()))
            {
                return node.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia);
            }

            _typeNames.Add(qualifiedName.ToString());

            return node;
        }
    }
}