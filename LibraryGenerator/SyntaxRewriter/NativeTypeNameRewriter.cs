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
        // TODO: This probably needs to be changed again
        //       Some types also contain their namespace
        //       We mainly want to filter out those that reference original source files
        if (nativeType.Contains("::"))
        {
            newType = null;
            return false;
        }

        // TODO: Add support for const pointer types
        // TODO: Add support for pointer types

        switch (nativeType)
        {
            case "uint8":
                nativeType = "byte";
                break;
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
            case "char *":
                nativeType = "string";
                break;
        }

        newType = SyntaxFactory.ParseTypeName(nativeType);

        if (newType.IsMissing)
        {
            newType = null;

            return false;
        }

        return true;
    }

    private static ParameterSyntax ExtractNativeTypeParameter(ParameterSyntax parameter, string nativeTypeName)
    {
        SyntaxToken newModifier = SyntaxFactory.Token(SyntaxKind.None);
        bool isConst = nativeTypeName.StartsWith("const");
        bool isRef = nativeTypeName.EndsWith("*");

        // TODO: Deal with 'type* const *' types

        if (isConst)
        {
            nativeTypeName = nativeTypeName[6..];
        }

        if (isRef)
        {
            nativeTypeName = nativeTypeName[..^2];
        }

        switch (nativeTypeName)
        {
            case "char":
                if (isRef)
                {
                    nativeTypeName = "string";
                }
                break;
            case "void":
                if (isRef && isConst)
                {
                    nativeTypeName = "byte[]";
                }
                break;
            case "int64":
                if (isRef && !isConst)
                {
                    nativeTypeName = "IntPtr";
                }
                break;
            case "uint64":
                if (isRef && !isConst)
                {
                    nativeTypeName = "UIntPtr";
                }
                break;
        }

        var nativeType = SyntaxFactory.ParseTypeName(nativeTypeName).WithTriviaFrom(parameter.Type!);

        if (isConst && isRef)
        {
            newModifier = SyntaxFactory.Token(SyntaxKind.InKeyword);
        }
        else if (isRef)
        {
            newModifier = SyntaxFactory.Token(SyntaxKind.RefKeyword);
        }

        if (nativeType.IsMissing)
        {
            Console.Error.WriteLine($"Couldn't find type of: {nativeType}");

            return parameter;
        }

        return isRef ? parameter.WithType(nativeType).AddModifiers(newModifier.WithTrailingTrivia(SyntaxFactory.Space)) : parameter.WithType(nativeType);
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
            ParameterSyntax newParameter = ExtractNativeTypeParameter(parameter, nativeTypeName);

            return parameter != newParameter ? newParameter.WithAttributeLists(parameter.AttributeLists.Remove(nativeTypeAttributeList)) : parameter;
        }

        if (!ExtractNativeType(nativeTypeName, out TypeSyntax paramNativeType))
        {
            return parameter;
        }

        return GetNativeTypeNode(parameter, paramNativeType).WithAttributeLists(
            parameter.AttributeLists.Remove(nativeTypeAttributeList));
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
        var attributeLists = node.AttributeLists;
        var replacedNode = node;

        if (nativeTypeAttributeList is not null && ExtractNativeType(GetNativeTypeName(nativeTypeAttributeList), out TypeSyntax nativeType))
        {
            attributeLists = attributeLists.Remove(nativeTypeAttributeList);
            replacedNode = replacedNode.WithReturnType(GetNativeTypeNode(node.ReturnType, nativeType));
        }

        return node.ReplaceNode(node,
            replacedNode.WithAttributeLists(attributeLists)
                .WithParameterList(node.ParameterList.RewriteParameterList(ShouldRewriteParameter, RewriteParameter)));
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