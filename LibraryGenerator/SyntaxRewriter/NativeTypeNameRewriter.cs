using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
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

    private static bool IsCallbackFunctionType(string nativeTypeName) => nativeTypeName.Contains('(') && nativeTypeName.Contains(')');

    private static string GetCSharpType(string nativeType)
    {
        switch (nativeType)
        {
            case "uint8":
                return "byte";
            case "uint64":
            case "size_t":
                return "ulong";
            case "uint32":
                return "uint";
            case "uint16":
                return "ushort";
            case "int64":
                return "long";
            case "char *":
                return "string";
            default:
                return nativeType;
        }
    }

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

        nativeType = GetCSharpType(nativeType);

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
        bool isPtrToConstPtr = nativeTypeName.EndsWith("*const *");
        bool isConst = nativeTypeName.StartsWith("const");
        bool isPtrToPtr = nativeTypeName.EndsWith("**");
        bool isPtr = nativeTypeName.EndsWith("*") && !isPtrToConstPtr;

        if (isConst)
        {
            nativeTypeName = nativeTypeName[6..];
        }

        if (isPtr)
        {
            nativeTypeName = nativeTypeName[..^2];
        }

        if (isPtrToPtr)
        {
            nativeTypeName = nativeTypeName[..^3];
            isPtr = true;
        }

        if (isPtrToConstPtr)
        {
            nativeTypeName = nativeTypeName[..^9];
        }

        switch (nativeTypeName)
        {
            case "char":
                if (isPtr)
                {
                    nativeTypeName = "string";
                }
                break;
            case "void":
                if (isPtr && isConst)
                {
                    nativeTypeName = "byte[]";
                }
                break;
            case "int64":
                if (isPtr && !isConst)
                {
                    nativeTypeName = "IntPtr";
                }
                break;
            case "uint64":
                if (isPtr && !isConst)
                {
                    nativeTypeName = "UIntPtr";
                }
                break;
            default:
                nativeTypeName = GetCSharpType(nativeTypeName);
                break;
        }

        if (isPtrToConstPtr || isPtrToPtr)
        {
            nativeTypeName = $"{nativeTypeName}[]";
        }

        var nativeType = SyntaxFactory.ParseTypeName(nativeTypeName).WithTriviaFrom(parameter.Type!);

        if (isConst && isPtr)
        {
            newModifier = SyntaxFactory.Token(SyntaxKind.InKeyword);
        }
        else if (isPtr)
        {
            newModifier = SyntaxFactory.Token(SyntaxKind.RefKeyword);
        }
        else if (isPtrToConstPtr)
        {
            newModifier = SyntaxFactory.Token(SyntaxKind.OutKeyword);
        }

        if (nativeType.IsMissing)
        {
            Console.Error.WriteLine($"Couldn't find type of: {nativeType}");

            return parameter;
        }

        if ((isPtr || isPtrToConstPtr) && !parameter.Modifiers.Any(modifier => modifier.IsKind(newModifier.Kind())))
        {
            return parameter.WithType(nativeType).AddModifiers(newModifier.WithTrailingTrivia(SyntaxFactory.Space));
        }

        return parameter.WithType(nativeType);
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

        if (IsCallbackFunctionType(nativeTypeName))
        {
            CallbackParameters.Add(parameter.GetReference());
            return parameter;
        }

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

        if (nativeTypeAttributeList is null || !ExtractNativeType(GetNativeTypeName(nativeTypeAttributeList), out TypeSyntax nativeType))
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