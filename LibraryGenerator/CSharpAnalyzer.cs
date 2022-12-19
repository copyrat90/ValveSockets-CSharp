using LibraryGenerator.SyntaxRewriter;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace LibraryGenerator;

public class CSharpAnalyzer
{
    private readonly CSharpCompilation _compilation;
    private readonly IReadOnlyCollection<CSharpSyntaxRewriter> _syntaxRewriter = new List<CSharpSyntaxRewriter>
    {
        new PragmaRemover()
    };

    public CSharpAnalyzer(string projectDirectory)
    {
        _compilation = CSharpCompilation.Create(Path.GetDirectoryName(projectDirectory));

        foreach (string filePath in Directory.EnumerateFiles(projectDirectory, "*.cs", SearchOption.AllDirectories))
        {
            Console.WriteLine($"\tAdding {Path.GetFileName(filePath)} to syntax tree.");
            SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(filePath), path: filePath);
            _compilation = _compilation.AddSyntaxTrees(tree);
        }
    }

    public void FixFiles()
    {
        foreach (CSharpSyntaxRewriter rewriter in _syntaxRewriter)
        {
            Console.WriteLine($"Running SyntaxRewriter: {rewriter}");

            foreach (SyntaxTree sourceTree in _compilation.SyntaxTrees)
            {
                var newSourceRoot = rewriter.Visit(sourceTree.GetRoot());

                if (newSourceRoot != sourceTree.GetRoot())
                {
                    using FileStream streamWriter = File.OpenWrite(sourceTree.FilePath);
                    using TextWriter writer = new StreamWriter(streamWriter);
                    newSourceRoot.WriteTo(writer);
                }
            }
        }
    }
}