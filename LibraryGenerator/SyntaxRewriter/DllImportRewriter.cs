using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace LibraryGenerator.SyntaxRewriter;

public class DllImportRewriter : CSharpSyntaxRewriter, ISyntaxRewriter
{
    private bool _hasCompilerServices;
    private bool _needsCompilerServices;

    private static bool AttributeIsDllImport(AttributeSyntax attribute) => attribute.Name.ToString() == "DllImport";

    public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var nodeAttributeLists = node.AttributeLists.FirstOrDefault(attributeList => attributeList.Attributes.Any(AttributeIsDllImport));

        if (nodeAttributeLists == null)
        {
            return node;
        }

        var nodeAttribute = nodeAttributeLists.Attributes.First(AttributeIsDllImport);

        IdentifierNameSyntax newName = SyntaxFactory.IdentifierName("LibraryImport");
        AttributeArgumentListSyntax newArgumentList = SyntaxFactory.AttributeArgumentList();
        SyntaxToken partialKeyword = SyntaxFactory.Token(SyntaxKind.PartialKeyword);

        var entryPointArg =
            nodeAttribute.ArgumentList!.Arguments.FirstOrDefault(argument => argument.ToString().StartsWith("EntryPoint"), null);

        if (entryPointArg != null)
        {
            newArgumentList = newArgumentList.AddArguments(nodeAttribute.ArgumentList!.Arguments.First(), entryPointArg);
        }
        else
        {
            newArgumentList = newArgumentList.AddArguments(nodeAttribute.ArgumentList!.Arguments.First());
        }

        SyntaxTokenList newModifiers = node.Modifiers;
        List<AttributeListSyntax> newAttributeLists = new()
        {
            SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList(new[]
            {
                nodeAttribute.WithName(newName).WithArgumentList(newArgumentList)
            })).WithTriviaFrom(nodeAttributeLists)
        };

        // TODO: Deal with CallingConvention

        foreach (AttributeListSyntax attributeList in node.AttributeLists)
        {
            if (!attributeList.Attributes.Contains(nodeAttribute))
            {
                newAttributeLists.Add(attributeList);
            }
        }

        foreach (var modifier in node.Modifiers)
        {
            if (modifier.IsKind(SyntaxKind.ExternKeyword))
            {
                newModifiers = node.Modifiers.Replace(modifier, partialKeyword.WithTriviaFrom(modifier));
                break;
            }
        }

        return node.ReplaceNode(node, node
            .WithModifiers(newModifiers)
            .WithAttributeLists(SyntaxFactory.List(newAttributeLists))
        );
    }

    public SyntaxNode FixupVisit(SyntaxNode node)
    {
        if (_needsCompilerServices)
        {
            foreach (var childNode in node.ChildNodes())
            {
                if (childNode.IsKind(SyntaxKind.UsingDirective))
                {
                    return node.InsertNodesAfter(
                        childNode, new []
                        {
                            SyntaxFactory.UsingDirective(
                                SyntaxFactory.ParseName("System.Runtime.CompilerServices").WithLeadingTrivia(SyntaxFactory.Space)
                            ).WithSemicolonToken(
                                SyntaxFactory.Token(SyntaxTriviaList.Empty, SyntaxKind.SemicolonToken, SyntaxFactory.TriviaList(SyntaxFactory.LineFeed))
                            )
                        }
                    );
                }
            }
        }

        return node;
    }
}