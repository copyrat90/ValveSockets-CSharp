using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryGenerator.SyntaxRewriter
{
    public class DllImportRewriter : CSharpSyntaxRewriter, ISyntaxRewriter
    {
        private bool _hasCompilerServices;
        private bool _needsCompilerServices;
        public bool NeedsFixupVisit => true;

        public SyntaxNode FixupVisit(SyntaxNode node)
        {
            if (_needsCompilerServices)
            {
                _needsCompilerServices = false;

                foreach (var childNode in node.ChildNodes())
                {
                    if (childNode.IsKind(SyntaxKind.UsingDirective))
                    {
                        return node.InsertNodesAfter(
                            childNode, new[]
                            {
                                SyntaxFactory.UsingDirective(
                                    SyntaxFactory.ParseName("System.Runtime.CompilerServices")
                                        .WithLeadingTrivia(SyntaxFactory.Space)
                                ).WithSemicolonToken(
                                    SyntaxFactory.Token(SyntaxTriviaList.Empty, SyntaxKind.SemicolonToken,
                                        SyntaxFactory.TriviaList(SyntaxFactory.LineFeed))
                                )
                            }
                        );
                    }
                }
            }

            _hasCompilerServices = false;

            return node;
        }

        private static bool AttributeIsDllImport(AttributeSyntax attribute) => attribute.Name.ToString() == "DllImport";

        private static bool TypeRequiresMarshalAsAttribute(TypeSyntax type) => type.ToString() switch
        {
            "bool" => true,
            "string" => true,
            _ => false
        };

        private static bool ShouldRewriteParameter(ParameterSyntax parameter) =>
            TypeRequiresMarshalAsAttribute(parameter.Type);

        private static ParameterSyntax RewriteParameter(ParameterSyntax parameter)
        {
            return parameter.WithAttributeLists(SyntaxFactory.List(new[]
            {
                SyntaxFactory.AttributeList(
                        SyntaxFactory.SeparatedList(new[] { GetMarshalAsAttribute(parameter.Type) }))
                    .WithTrailingTrivia(SyntaxFactory.Space).WithTrailingTrivia(SyntaxFactory.Space)
            }));
        }

        private static AttributeSyntax GetMarshalAsAttribute(TypeSyntax type)
        {
            if (!TypeRequiresMarshalAsAttribute(type))
            {
                return null;
            }

            var argumentList = SyntaxFactory.SeparatedList<AttributeArgumentSyntax>();
            var attribute = SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("MarshalAs"));
            var unmanagedTypeName = SyntaxFactory.IdentifierName("UnmanagedType");

            switch (type.ToString())
            {
                case "bool":
                    argumentList = argumentList.Add(SyntaxFactory.AttributeArgument(
                        SyntaxFactory.QualifiedName(unmanagedTypeName, SyntaxFactory.IdentifierName("Bool")))
                    );
                    break;
                case "string":
                    argumentList = argumentList.Add(SyntaxFactory.AttributeArgument(
                        SyntaxFactory.QualifiedName(unmanagedTypeName, SyntaxFactory.IdentifierName("LPUTF8Str")))
                    );
                    break;
            }

            return attribute.WithArgumentList(SyntaxFactory.AttributeArgumentList(argumentList));
        }

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
            var nodeAttributeLists =
                node.AttributeLists.FirstOrDefault(attributeList => attributeList.Attributes.Any(AttributeIsDllImport));

            if (nodeAttributeLists == null)
            {
                return node;
            }

            AttributeListSyntax lastAttribute = node.AttributeLists.Last();
            var attributeTrailingTrivia = lastAttribute.GetTrailingTrivia();
            var attributeLeadingTrivia = lastAttribute.GetLeadingTrivia();

            if (attributeLeadingTrivia.First().IsKind(SyntaxKind.EndOfLineTrivia))
            {
                attributeLeadingTrivia = attributeLeadingTrivia.RemoveAt(0);
            }

            var nodeAttribute = nodeAttributeLists.Attributes.First(AttributeIsDllImport);

            IdentifierNameSyntax newName = SyntaxFactory.IdentifierName("LibraryImport");
            AttributeArgumentListSyntax newArgumentList = SyntaxFactory.AttributeArgumentList();
            SyntaxToken partialKeyword = SyntaxFactory.Token(SyntaxKind.PartialKeyword);

            var entryPointArg =
                nodeAttribute.ArgumentList!.Arguments.FirstOrDefault(
                    argument => argument.ToString().StartsWith("EntryPoint"), null);

            var callConvArg =
                nodeAttribute.ArgumentList!.Arguments.FirstOrDefault(
                    argument => argument.ToString().StartsWith("CallingConvention"), null);

            if (entryPointArg != null)
            {
                newArgumentList =
                    newArgumentList.AddArguments(nodeAttribute.ArgumentList!.Arguments.First(), entryPointArg);
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

                    newAttributeLists.Add(SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList(new[]
                        {
                            SyntaxFactory.Attribute(callConvAttributeName,
                                SyntaxFactory.AttributeArgumentList(SyntaxFactory.SeparatedList(new[]
                                {
                                    SyntaxFactory.AttributeArgument(callConvsArgumentName, null, callConvsArgumentValue)
                                })))
                        })).WithTrailingTrivia(attributeTrailingTrivia)
                        .WithLeadingTrivia(attributeLeadingTrivia));
                }
            }

            if (TypeRequiresMarshalAsAttribute(node.ReturnType))
            {
                newAttributeLists.Add(SyntaxFactory.AttributeList(
                        SyntaxFactory.AttributeTargetSpecifier(SyntaxFactory.Token(SyntaxKind.ReturnKeyword))
                            .WithTrailingTrivia(SyntaxFactory.Space),
                        SyntaxFactory.SeparatedList(new[] { GetMarshalAsAttribute(node.ReturnType) }))
                    .WithTrailingTrivia(attributeTrailingTrivia)
                    .WithLeadingTrivia(attributeLeadingTrivia));
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

            var newParameters = node.ParameterList.RewriteParameterList(ShouldRewriteParameter, RewriteParameter);

            if (node.ReturnType.IsKind(SyntaxKind.PointerType))
            {
                return node.ReplaceNode(node, node
                    .WithModifiers(newModifiers).WithReturnType(SyntaxFactory.ParseTypeName("IntPtr").WithTriviaFrom(node.ReturnType))
                    .WithAttributeLists(SyntaxFactory.List(newAttributeLists)).WithParameterList(newParameters)
                );
            }

            return node.ReplaceNode(node, node
                .WithModifiers(newModifiers)
                .WithAttributeLists(SyntaxFactory.List(newAttributeLists)).WithParameterList(newParameters)
            );
        }
    }
}