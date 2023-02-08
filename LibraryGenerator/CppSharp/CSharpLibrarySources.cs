using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using System.Collections.Generic;

namespace LibraryGenerator.CppSharp
{
    public class CSharpLibrarySources : CSharpSources
    {
        public CSharpLibrarySources(BindingContext context) : base(context) { }

        public CSharpLibrarySources(BindingContext context, IEnumerable<TranslationUnit> units) : base(context, units) { }

        public override void GenerateComment(RawComment comment)
        {
            if (comment.FullComment != null)
            {
                PushBlock(BlockKind.BlockComment);
                ActiveBlock.Text.Print(comment.FullComment, DocumentationCommentKind);
                PopBlock();
                return;
            }

            base.GenerateComment(comment);
        }
    }
}