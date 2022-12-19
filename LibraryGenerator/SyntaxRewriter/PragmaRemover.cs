using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LibraryGenerator.SyntaxRewriter;

public class PragmaRemover : CSharpSyntaxRewriter
{
    public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
    {
        if (trivia.IsDirective && trivia.ToString().StartsWith("#pragma warning disable CS0109"))
        {
            return new SyntaxTrivia();
        }

        return trivia;
    }
}