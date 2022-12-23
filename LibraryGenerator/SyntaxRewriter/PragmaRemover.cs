using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;

namespace LibraryGenerator.SyntaxRewriter;

public class PragmaRemover : CSharpSyntaxRewriter, ISyntaxRewriter
{
    public bool NeedsFixupVisit => false;

    public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
    {
        if (trivia.IsDirective && trivia.ToString().StartsWith("#pragma warning disable CS0109"))
        {
            return new SyntaxTrivia();
        }

        return trivia;
    }

    public SyntaxNode FixupVisit(SyntaxNode node)
    {
        throw new NotImplementedException();
    }
}