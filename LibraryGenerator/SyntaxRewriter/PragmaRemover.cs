using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LibraryGenerator.SyntaxRewriter;

public class PragmaRemover : CSharpSyntaxRewriter, ISyntaxRewriter
{
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
        return node;
    }
}