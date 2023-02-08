using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace LibraryGenerator.SyntaxRewriter
{
    public class OperatorRewriter : CSharpSyntaxRewriter, ISyntaxRewriter
    {
        public bool NeedsFixupVisit => false;

        public SyntaxNode FixupVisit(SyntaxNode node)
        {
            throw new NotImplementedException();
        }

        public override SyntaxNode VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            return node.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia);
        }

        public override SyntaxNode VisitBlock(BlockSyntax node)
        {
            if (node.ToString().Contains(".operator"))
            {
                SyntaxList<StatementSyntax> statements = SyntaxFactory.List<StatementSyntax>();
                bool skipStatement = false;

                foreach (var statement in node.Statements)
                {
                    if (statement.ToFullString().EndsWith("\n"))
                    {
                        if (!skipStatement)
                        {
                            statements = statements.Add(statement);
                        }

                        skipStatement = false;
                    }
                    else if (statement.ToFullString().Contains(".operator"))
                    {
                        skipStatement = !statement.ToFullString().EndsWith("\n");
                    }
                }

                return node.ReplaceNode(node, node.WithStatements(statements));
            }

            return node;
        }
    }
}