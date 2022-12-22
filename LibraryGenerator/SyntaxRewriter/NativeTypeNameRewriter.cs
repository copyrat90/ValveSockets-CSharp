using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace LibraryGenerator.SyntaxRewriter;

public class NativeTypeNameRewriter : CSharpSyntaxRewriter, ISyntaxRewriter
{
    private static TypeSyntax ExtractNativeType(AttributeArgumentSyntax argument)
    {
        var nativeType = argument.Expression.ToString().Trim('"');

        // TODO: Add support for const pointer types
        // TODO: Add support for pointer types

        switch (nativeType)
        {
            case "uint64":
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

        return SyntaxFactory.ParseTypeName(nativeType);
    }

    private static bool TryGetNativeTypeName(SyntaxList<AttributeListSyntax> attributeLists, out TypeSyntax nativeType)
    {
        nativeType = null;

        foreach (AttributeListSyntax attributeList in attributeLists)
        {
            AttributeSyntax nativeTypeAttribute = attributeList.Attributes.FirstOrDefault(attribute => attribute.Name.ToString() == "NativeTypeName", null);

            if (nativeTypeAttribute != null)
            {
                nativeType = ExtractNativeType(nativeTypeAttribute.ArgumentList!.Arguments.First());

                return true;
            }
        }

        return false;
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

        if (currentType.IsKind(SyntaxKind.OperatorDeclaration) && currentType is OperatorDeclarationSyntax currentOperator)
        {
            return currentOperator.WithReturnType(GetNativeTypeNode(currentOperator.ReturnType, nativeType)) as T;
        }

        if (currentType.IsKind(SyntaxKind.FieldDeclaration) && currentType is FieldDeclarationSyntax currentField)
        {
            return currentField.WithDeclaration(
                currentField.Declaration.WithType(GetNativeTypeNode(currentField.Declaration.Type, nativeType))) as T;
        }

        if (currentType.IsKind(SyntaxKind.PropertyDeclaration) && currentType is PropertyDeclarationSyntax currentProperty)
        {
            return currentProperty.WithType(GetNativeTypeNode(currentProperty.Type, nativeType)) as T;
        }

        return currentType;
    }

    public override SyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node)
    {
        if (!TryGetNativeTypeName(node.AttributeLists, out TypeSyntax nativeType))
        {
            return node;
        }

        // node.BaseList.Types is a nightmare.

        // Console.WriteLine($"\tEnums are not supported. {node.Identifier} currentType: {node.BaseList?.Types.FirstOrDefault()} nativeType: {nativeType}");

        return node;
    }

    public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
    {
        if (!TryGetNativeTypeName(node.AttributeLists, out TypeSyntax nativeType))
        {
            return node;
        }

        return node.ReplaceNode(node, GetNativeTypeNode(node, nativeType));
    }

    public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        if (TryGetNativeTypeName(node.AttributeLists, out TypeSyntax nativeType))
        {
            node = node.ReplaceNode(node, node.WithReturnType(GetNativeTypeNode(node.ReturnType, nativeType)));
        }

        foreach (var childNode in node.ChildNodes())
        {
            if (childNode.IsKind(SyntaxKind.ParameterList) && childNode is ParameterListSyntax parameterList)
            {
                bool parameterListChanged = false;
                var newParameterList = SyntaxFactory.SeparatedList<ParameterSyntax>();

                foreach (ParameterSyntax parameter in parameterList.Parameters)
                {
                    if (!TryGetNativeTypeName(parameter.AttributeLists, out TypeSyntax paramNativeType))
                    {
                        newParameterList = newParameterList.Add(parameter);
                        continue;
                    }

                    var newParameter = GetNativeTypeNode(parameter, paramNativeType);

                    if (newParameter != parameter)
                    {
                        newParameterList = newParameterList.Add(newParameter);
                        parameterListChanged = true;
                    }
                    else
                    {
                        newParameterList = newParameterList.Add(parameter);
                    }
                }

                if (parameterListChanged)
                {
                    node = node.ReplaceNode(parameterList, parameterList.WithParameters(newParameterList));
                }
            }
        }

        return node;
    }

    public override SyntaxNode VisitOperatorDeclaration(OperatorDeclarationSyntax node)
    {
        if (!TryGetNativeTypeName(node.AttributeLists, out TypeSyntax nativeType))
        {
            return node;
        }

        return node.ReplaceNode(node, GetNativeTypeNode(node, nativeType));
    }

    public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        if (!TryGetNativeTypeName(node.AttributeLists, out TypeSyntax nativeType))
        {
            return node;
        }

        return node.ReplaceNode(node, GetNativeTypeNode(node, nativeType));
    }

    public override SyntaxNode VisitParameter(ParameterSyntax node)
    {
        if (!TryGetNativeTypeName(node.AttributeLists, out TypeSyntax nativeType))
        {
            return node;
        }

        return node.ReplaceNode(node, GetNativeTypeNode(node, nativeType));
    }

    public SyntaxNode FixupVisit(SyntaxNode node)
    {
        return node;
    }
}