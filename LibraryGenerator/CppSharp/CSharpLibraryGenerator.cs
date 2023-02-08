using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using System.Collections.Generic;

namespace LibraryGenerator.CppSharp
{
    public class CSharpLibraryGenerator : CSharpGenerator
    {
        private readonly CSharpTypePrinter typePrinter;

        public CSharpLibraryGenerator(BindingContext context) : base(context)
        {
            typePrinter = new CSharpTypePrinter(context);
        }

        public override List<CodeGenerator> Generate(IEnumerable<TranslationUnit> units)
        {
            var outputs = new List<CodeGenerator>();

            var gen = new CSharpLibrarySources(Context, units) { TypePrinter = typePrinter };
            outputs.Add(gen);

            return outputs;
        }

        protected override string TypePrinterDelegate(Type type)
        {
            return type.Visit(typePrinter);
        }
    }
}