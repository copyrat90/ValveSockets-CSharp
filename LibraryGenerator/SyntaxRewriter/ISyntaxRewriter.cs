#nullable enable
using Microsoft.CodeAnalysis;

namespace LibraryGenerator.SyntaxRewriter;

public interface ISyntaxRewriter
{
    public SyntaxNode? Visit(SyntaxNode? node);
    public SyntaxNode? FixupVisit(SyntaxNode? node);
}