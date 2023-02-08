#nullable enable
using Microsoft.CodeAnalysis;

namespace LibraryGenerator.SyntaxRewriter
{
    public interface ISyntaxRewriter
    {
        public bool NeedsFixupVisit { get; }

        public SyntaxNode? Visit(SyntaxNode? node);
        public SyntaxNode? FixupVisit(SyntaxNode? node);
    }
}