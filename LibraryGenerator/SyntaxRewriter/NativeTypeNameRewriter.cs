using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace LibraryGenerator.SyntaxRewriter;

public class NativeTypeNameRewriter : CSharpSyntaxRewriter, ISyntaxRewriter
{
    public bool NeedsFixupVisit => false;

    public SyntaxNode FixupVisit(SyntaxNode node)
    {
        throw new NotImplementedException();
    }

    private static AttributeListSyntax GetNativeTypeAttributeList(SyntaxList<AttributeListSyntax> attributeLists) =>
        attributeLists.FirstOrDefault(attributeList =>
            attributeList.Attributes.Any(attribute => attribute.Name.ToString() == "NativeTypeName"));

    private static string GetNativeTypeName(AttributeListSyntax attributeList) =>
        attributeList.Attributes.First(attribute => attribute.Name.ToString() == "NativeTypeName").ArgumentList!
            .Arguments.First().Expression.ToString().Trim('"');

    private static bool IsSpecialNativeType(string nativeTypeName) =>
        nativeTypeName.Contains('*') || nativeTypeName.Contains("const") || nativeTypeName.Contains('&');

    private static bool ExtractNativeType(string nativeType, out TypeSyntax newType)
    {
        // TODO: Add support for const pointer types
        // TODO: Add support for pointer types

        switch (nativeType)
        {
            case "uint64":
            case "size_t":
                nativeType = "ulong";
                break;
            case "uint32":
                nativeType = "uint";
                break;
            case "uint16":
                nativeType = "ushort";
                break;
            case "int64":
                nativeType = "long";
                break;
        }

        newType = SyntaxFactory.ParseTypeName(nativeType);

        if (IsSpecialNativeType(nativeType) || newType.IsKind(SyntaxKind.IdentifierName))
        {
            newType = null;

            return false;
        }

        return true;
    }

    private static T GetNativeTypeNode<T>(T currentType, TypeSyntax nativeType) where T : SyntaxNode
    {
        if (currentType.IsKind(SyntaxKind.PredefinedType) && nativeType.IsKind(SyntaxKind.PredefinedType))
        {
            if (currentType.ToString() != nativeType.ToString())
            {
                return nativeType.WithTriviaFrom(currentType) as T;
            }

            return currentType;
        }

        // TODO: Add support for IdentifierName
        // TODO: Add support for PointerType

        if (currentType.IsKind(SyntaxKind.Parameter) && currentType is ParameterSyntax currentParameter)
        {
            return currentParameter.WithType(GetNativeTypeNode(currentParameter.Type, nativeType)) as T;
        }

        if (currentType.IsKind(SyntaxKind.OperatorDeclaration) &&
            currentType is OperatorDeclarationSyntax currentOperator)
        {
            return currentOperator.WithReturnType(GetNativeTypeNode(currentOperator.ReturnType, nativeType)) as T;
        }

        if (currentType.IsKind(SyntaxKind.FieldDeclaration) && currentType is FieldDeclarationSyntax currentField)
        {
            return currentField.WithDeclaration(
                currentField.Declaration.WithType(GetNativeTypeNode(currentField.Declaration.Type, nativeType))) as T;
        }

        if (currentType.IsKind(SyntaxKind.PropertyDeclaration) &&
            currentType is PropertyDeclarationSyntax currentProperty)
        {
            return currentProperty.WithType(GetNativeTypeNode(currentProperty.Type, nativeType)) as T;
        }

        return currentType;
    }

    private static bool ShouldRewriteParameter(ParameterSyntax parameter) =>
        GetNativeTypeAttributeList(parameter.AttributeLists) is not null;

    private static ParameterSyntax RewriteParameter(ParameterSyntax parameter)
    {
        AttributeListSyntax nativeTypeAttributeList = GetNativeTypeAttributeList(parameter.AttributeLists);
        string nativeTypeName = GetNativeTypeName(nativeTypeAttributeList);

        if (IsSpecialNativeType(nativeTypeName))
        {
            // TODO: Extract "special" native types
        }
        else
        {
            if (!ExtractNativeType(nativeTypeName, out TypeSyntax paramNativeType))
            {
                return parameter;
            }

            return GetNativeTypeNode(parameter, paramNativeType).WithAttributeLists(
                parameter.AttributeLists.Remove(nativeTypeAttributeList));
        }

        return parameter;
    }

    public override SyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node)
    {
        var nativeTypeAttributeList = GetNativeTypeAttributeList(node.AttributeLists);

        if (nativeTypeAttributeList is null)
        {
            return node;
        }

        // node.BaseList.Types is a nightmare.

        // Console.WriteLine($"\tEnums are not supported. {node.Identifier} currentType: {node.BaseList?.Types.FirstOrDefault()} nativeType: {nativeType}");

        return node;
    }

    public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
    {
        var nativeTypeAttributeList = GetNativeTypeAttributeList(node.AttributeLists);

        if (nativeTypeAttributeList is null ||
            !ExtractNativeType(GetNativeTypeName(nativeTypeAttributeList), out TypeSyntax nativeType))
        {
            return node;
        }

        var attributeLists = node.AttributeLists.Remove(nativeTypeAttributeList);

        return node.ReplaceNode(node, GetNativeTypeNode(node, nativeType).WithAttributeLists(attributeLists));
    }

    public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var nativeTypeAttributeList = GetNativeTypeAttributeList(node.AttributeLists);

        if (nativeTypeAttributeList is null ||
            !ExtractNativeType(GetNativeTypeName(nativeTypeAttributeList), out TypeSyntax nativeType))
        {
            return node;
        }

        var methodAttributeLists = node.AttributeLists.Remove(nativeTypeAttributeList);

        node = node.ReplaceNode(node,
            node.WithReturnType(GetNativeTypeNode(node.ReturnType, nativeType))
                .WithAttributeLists(methodAttributeLists));

        foreach (var childNode in node.ChildNodes())
        {
            if (childNode.IsKind(SyntaxKind.ParameterList) && childNode is ParameterListSyntax parameterList)
            {
                node = node.ReplaceNode(parameterList,
                    parameterList.RewriteParameterList(ShouldRewriteParameter, RewriteParameter));
            }
        }

        return node;
    }

    public override SyntaxNode VisitOperatorDeclaration(OperatorDeclarationSyntax node)
    {
        var nativeTypeAttributeList = GetNativeTypeAttributeList(node.AttributeLists);

        if (nativeTypeAttributeList is null ||
            !ExtractNativeType(GetNativeTypeName(nativeTypeAttributeList), out TypeSyntax nativeType))
        {
            return node;
        }

        var attributeLists = node.AttributeLists.Remove(nativeTypeAttributeList);

        return node.ReplaceNode(node, GetNativeTypeNode(node, nativeType).WithAttributeLists(attributeLists));
    }

    public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        var nativeTypeAttributeList = GetNativeTypeAttributeList(node.AttributeLists);

        if (nativeTypeAttributeList is null ||
            !ExtractNativeType(GetNativeTypeName(nativeTypeAttributeList), out TypeSyntax nativeType))
        {
            return node;
        }

        var attributeLists = node.AttributeLists.Remove(nativeTypeAttributeList);

        return node.ReplaceNode(node, GetNativeTypeNode(node, nativeType).WithAttributeLists(attributeLists));
    }

    public override SyntaxNode VisitParameter(ParameterSyntax node)
    {
        var nativeTypeAttributeList = GetNativeTypeAttributeList(node.AttributeLists);

        if (nativeTypeAttributeList is null ||
            !ExtractNativeType(GetNativeTypeName(nativeTypeAttributeList), out TypeSyntax nativeType))
        {
            return node;
        }

        var attributeLists = node.AttributeLists.Remove(nativeTypeAttributeList);

        return node.ReplaceNode(node, GetNativeTypeNode(node, nativeType).WithAttributeLists(attributeLists));
    }
}