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

    };

    public CSharpAnalyzer(string projectDirectory)
    {
        _compilation = CSharpCompilation.Create(Path.GetDirectoryName(projectDirectory));

        foreach (string filePath in Directory.EnumerateFiles(projectDirectory, "*.cs", SearchOption.AllDirectories))
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(filePath));
            _compilation.AddSyntaxTrees(tree);
        }
    }

    public void FixFiles()
    {
        foreach (CSharpSyntaxRewriter rewriter in _syntaxRewriter)
        {
            Console.WriteLine($"Running SyntaxRewriter: {rewriter}");

            foreach (SyntaxTree sourceTree in _compilation.SyntaxTrees)
            {
                rewriter.Visit(sourceTree.GetRoot());
            }
        }
    }
}