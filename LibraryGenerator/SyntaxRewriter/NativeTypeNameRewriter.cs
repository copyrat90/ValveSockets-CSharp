using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LibraryGenerator.SyntaxRewriter;

public class NativeTypeNameRewriter : CSharpSyntaxRewriter, ISyntaxRewriter
{
    private static readonly Regex CallbackRegex = new(@"(.*)\(\*\)\((.*)\)");

    private static readonly List<SyntaxReference> CallbackParameters = new();

    public bool NeedsFixupVisit => CallbackParameters.Count > 0;

    public SyntaxNode FixupVisit(SyntaxNode node)
    {
        HashSet<string> genCallbackFunctions = new();
        ClassDeclarationSyntax classDeclaration = null;
        SyntaxList<MemberDeclarationSyntax> classMembers = new();

        foreach (SyntaxReference reference in CallbackParameters)
        {
            ParameterSyntax referenceParam = (reference.GetSyntax() as ParameterSyntax)!;

            foreach (var descendantNode in node.DescendantNodes())
            {
                if (descendantNode.IsKind(SyntaxKind.ClassDeclaration))
                {
                    classDeclaration = (descendantNode as ClassDeclarationSyntax)!;
                }

                if (descendantNode.IsKind(SyntaxKind.Parameter) && referenceParam.ToString() == descendantNode.ToString())
                {
                    ParameterSyntax parameter = (descendantNode as ParameterSyntax)!;
                    MethodDeclarationSyntax methodDeclaration = (parameter.Parent!.Parent as MethodDeclarationSyntax)!;
                    AttributeListSyntax nativeTypeAttributeList = GetNativeTypeAttributeList(parameter.AttributeLists);
                    string nativeTypeName = GetNativeTypeName(nativeTypeAttributeList);

                    Match match = CallbackRegex.Match(nativeTypeName);
                    string returnType = match.Groups[1].Value.Trim();
                    string[] parameters = match.Groups[2].Value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    string callbackFunctionName = parameter.Identifier.ToString().FirstToUpper();

                    if (!genCallbackFunctions.Contains(nativeTypeName))
                    {
                        var newDelegate = GenerateDelegate(callbackFunctionName, parameters, returnType).WithTriviaFrom(methodDeclaration);
                        classMembers = classMembers.Add(newDelegate);

                        genCallbackFunctions.Add(nativeTypeName);
                    }

                    node = node.ReplaceNode(parameter, parameter.WithType(SyntaxFactory.ParseTypeName(callbackFunctionName).WithTriviaFrom(parameter.Type!))
                        .WithAttributeLists(parameter.AttributeLists.Remove(nativeTypeAttributeList)));
                }
            }
        }

        if (classDeclaration is not null && classMembers.Count != 0)
        {
            // FIXME: It seems like classDeclaration is no longer valid after the first replacements have been made?
            //        But there has to be a better way to do this.
            //        This cost me way too much time and I would have expected an exception
            //        if the node which is about to be replaced can't be found.

            node = node.InsertNodesBefore(
                node.ChildNodes().ToArray()[^1].ChildNodes().ToArray()[^1],
                new [] {classDeclaration.WithMembers(classMembers).WithTrailingTrivia(SyntaxFactory.LineFeed, SyntaxFactory.LineFeed)});
        }

        return node;
    }

    private static DelegateDeclarationSyntax GenerateDelegate(string name, string[] parameters, string returnType)
    {
        var parameterList = SyntaxFactory.SeparatedList<ParameterSyntax>();

        int paramNum = 1;
        foreach (string parameter in parameters)
        {
            parameterList = parameterList.Add(ExtractNativeTypeParameter(
                SyntaxFactory.Parameter(SyntaxFactory.Identifier($"param{paramNum}")),
                parameter));

            paramNum++;
        }


        return SyntaxFactory.DelegateDeclaration(
            SyntaxFactory.List<AttributeListSyntax>(),
            SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
            SyntaxFactory.Token(SyntaxFactory.TriviaList(SyntaxFactory.Space), SyntaxKind.DelegateKeyword, SyntaxFactory.TriviaList(SyntaxFactory.Space)),
            SyntaxFactory.ParseTypeName(GetCSharpType(returnType)).WithTrailingTrivia(SyntaxFactory.Space),
            SyntaxFactory.Identifier(name),
            null, SyntaxFactory.ParameterList(parameterList),
            SyntaxFactory.List<TypeParameterConstraintClauseSyntax>(),
            SyntaxFactory.Token(SyntaxKind.SemicolonToken)
        );
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

    private static bool IsSpecialTypePtrToConstPtr(string nativeType) => nativeType.EndsWith("*const *");
    private static bool IsSpecialTypeConst(string nativeType) => nativeType.StartsWith("const");
    private static bool IsSpecialTypePtrToPtr(string nativeType) => nativeType.EndsWith("**");
    private static bool IsSpecialTypePtr(string nativeType) => nativeType.EndsWith("*") && !IsSpecialTypePtrToPtr(nativeType);

    private static string GetCSharpType(string nativeType)
    {
        if (IsSpecialNativeType(nativeType))
        {
            bool isConst = IsSpecialTypeConst(nativeType);
            bool isPtr = IsSpecialTypePtr(nativeType);
            bool isPtrToPtr = IsSpecialTypePtrToPtr(nativeType);
            bool isPtrToConstPtr = IsSpecialTypePtrToConstPtr(nativeType);

            if (isConst)
            {
                nativeType = nativeType[6..];
            }

            if (isPtr)
            {
                nativeType = nativeType[..^2];
            }

            if (isPtrToPtr)
            {
                nativeType = nativeType[..^3];
                isPtr = true;
            }

            if (isPtrToConstPtr)
            {
                nativeType = nativeType[..^9];
            }


            switch (nativeType)
            {
                case "char":
                    if (isPtr)
                    {
                        return "string";
                    }
                    break;
                case "void":
                    if (isPtr && isConst)
                    {
                        return "byte[]";
                    }
                    else if (isPtr)
                    {
                        return "IntPtr";
                    }
                    break;
                case "int64":
                    if (isPtr && !isConst)
                    {
                        return "IntPtr";
                    }
                    break;
                case "uint64":
                    if (isPtr && !isConst)
                    {
                        return "UIntPtr";
                    }
                    break;
            }
        }

        switch (nativeType)
        {
            case "uint8":
                return "byte";
            case "uint64":
            case "size_t":
                return "ulong";
            case "uint32":
                return "uint";
            case "int32":
                return "int";
            case "uint16":
                return "ushort";
            case "int64":
                return "long";
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

        newType = SyntaxFactory.ParseTypeName(GetCSharpType(nativeType));

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

        bool isConst = IsSpecialTypeConst(nativeTypeName);
        bool isPtr = IsSpecialTypePtr(nativeTypeName);
        bool isPtrToPtr = IsSpecialTypePtrToPtr(nativeTypeName);
        bool isPtrToConstPtr = IsSpecialTypePtrToConstPtr(nativeTypeName);

        if (isPtrToConstPtr || isPtrToPtr)
        {
            nativeTypeName = $"{nativeTypeName}[]";
        }

        var nativeType = SyntaxFactory.ParseTypeName(GetCSharpType(nativeTypeName));

        if (parameter.Type is not null)
        {
            nativeType = nativeType.WithTriviaFrom(parameter.Type);
        }
        else
        {
            nativeType = nativeType.WithTrailingTrivia(SyntaxFactory.Space);
        }

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
            Console.Error.WriteLine($"Couldn't find type of: {nativeTypeName}");

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

        Console.WriteLine($"\tEnums are not supported. {node.Identifier} currentType: {node.BaseList?.Types.FirstOrDefault()}");

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