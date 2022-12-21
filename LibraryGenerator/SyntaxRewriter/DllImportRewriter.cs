using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryGenerator.SyntaxRewriter;

public class DllImportRewriter : CSharpSyntaxRewriter, ISyntaxRewriter
{
    private bool _hasCompilerServices;
    private bool _needsCompilerServices;

    private static bool AttributeIsDllImport(AttributeSyntax attribute) => attribute.Name.ToString() == "DllImport";

    public override SyntaxNode VisitUsingDirective(UsingDirectiveSyntax node)
    {
        if (node.Name.ToString() == "System.Runtime.CompilerServices")
        {
            _hasCompilerServices = true;
        }

        return node;
    }

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

        var callConvArg =
            nodeAttribute.ArgumentList!.Arguments.FirstOrDefault(argument => argument.ToString().StartsWith("CallingConvention"), null);

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

        if (callConvArg != null)
        {
            NameSyntax callConvAttributeName = SyntaxFactory.IdentifierName("UnmanagedCallConv");
            NameEqualsSyntax callConvsArgumentName = SyntaxFactory.NameEquals("CallConvs");
            ExpressionSyntax callConvsArgumentValue = null;

            switch (callConvArg.Expression.ToString())
            {
                case "__CallingConvention.Cdecl":
                case "CallingConvention.Cdecl":
                    callConvsArgumentValue = SyntaxFactory.ParseExpression("new [] { typeof(CallConvCdecl) }");
                    break;

                case "CallingConvention.ThisCall":
                    callConvsArgumentValue = SyntaxFactory.ParseExpression("new [] { typeof(CallConvThiscall) }");
                    break;

                default:
                    Console.WriteLine($"\tNo conversion found for calling convention: {callConvArg.Expression}");
                    break;
            }

            if (callConvsArgumentValue != null)
            {
                if (!_hasCompilerServices)
                {
                    _needsCompilerServices = true;
                }

                var lastAttribute = node.AttributeLists.Last();
                var lastAttributeLeadingTrivia = lastAttribute.GetLeadingTrivia();

                if (lastAttributeLeadingTrivia.First().IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    lastAttributeLeadingTrivia = lastAttributeLeadingTrivia.RemoveAt(0);
                }

                newAttributeLists.Add(SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList(new[]
                {
                    SyntaxFactory.Attribute(callConvAttributeName, SyntaxFactory.AttributeArgumentList(SyntaxFactory.SeparatedList(new []
                    {
                        SyntaxFactory.AttributeArgument(callConvsArgumentName, null, callConvsArgumentValue)
                    })))
                })).WithTrailingTrivia(lastAttribute.GetTrailingTrivia()).WithLeadingTrivia(lastAttributeLeadingTrivia));
            }
        }

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