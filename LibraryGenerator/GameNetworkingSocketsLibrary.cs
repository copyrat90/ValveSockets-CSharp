using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using System.Collections.Generic;
using System.IO;
using CppSharp.Parser;

namespace LibraryGenerator;

public class GameNetworkingSocketsLibrary : ILibrary
{
    private const string DotNetLibraryName = "Valve.Sockets";
    private const string LibraryName = "GameNetworkingSockets";

    private readonly List<string> _headerFiles;
    private readonly string _steamIncludePath;
    private readonly string _outputPath;

    public GameNetworkingSocketsLibrary(string repoPath, string outputPath)
    {
        _steamIncludePath = Path.Combine(repoPath, "include", "steam");
        _outputPath = outputPath;

        _headerFiles = new List<string>
        {
            Path.Combine(_steamIncludePath, "isteamnetworkingsockets.h"),
            Path.Combine(_steamIncludePath, "isteamnetworkingmessages.h"),
            Path.Combine(_steamIncludePath, "isteamnetworkingutils.h"),
        };
    }

    public void Setup(Driver driver)
    {
        driver.ParserOptions.LanguageVersion = LanguageVersion.CPP11_GNU;
        driver.ParserOptions.SkipPrivateDeclarations = true;
        driver.ParserOptions.MicrosoftMode = false;
        driver.ParserOptions.TargetTriple = "x86_64-linux-gnu";

        driver.Options.GeneratorKind = GeneratorKind.CSharp;
        driver.Options.GenerationOutputMode = GenerationOutputMode.FilePerUnit;
        driver.Options.GenerateDeprecatedDeclarations = true;
        driver.Options.StripLibPrefix = true;
        driver.Options.CheckSymbols = true;
        driver.Options.UseSpan = true;
        driver.Options.GenerateDefaultValuesForArguments = true;
        driver.Options.MarshalCharAsManagedChar = true;

        driver.Options.OutputDir = _outputPath;

        // driver.Options.GenerateDebugOutput = true;
        // driver.Options.Verbose = true;

        var module = driver.Options.AddModule(LibraryName);
        module.OutputNamespace = DotNetLibraryName;

        module.IncludeDirs.Add(_steamIncludePath);
        module.Headers.AddRange(_headerFiles);
    }

    public void SetupPasses(Driver driver)
    {

    }

    public void Preprocess(Driver driver, ASTContext ctx)
    {

    }

    public void Postprocess(Driver driver, ASTContext ctx)
    {
        foreach (var unit in ctx.TranslationUnits)
        {
            foreach (var unitClass in unit.Classes)
            {
                if (unitClass.Name.StartsWith("I"))
                {
                    unitClass.Type = ClassType.Interface;
                    unitClass.IsAbstract = false;
                }
            }
        }
    }

    public void GenerateCode(Driver driver, List<GeneratorOutput> outputs)
    {
        foreach (var output in outputs)
        {
            if (!output.TranslationUnit.FileName.StartsWith("i"))
            {
                output.Outputs.Clear();
            }
        }
    }
}