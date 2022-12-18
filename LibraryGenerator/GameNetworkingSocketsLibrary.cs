using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using System.Collections.Generic;
using System;
using System.IO;
using CppSharp.Parser;

namespace LibraryGenerator;

public class GameNetworkingSocketsLibrary : ILibrary
{
    private const string DotNetLibraryName = "Valve.Sockets";
    private const string LibraryName = "GameNetworkingSockets";

    private readonly string _outputPath;

    private readonly string _includePath;
    private readonly string _steamIncludePath;
    private readonly string _sourcePath;
    private readonly string _libraryPath;
    private readonly List<string> _headerFiles;
    private readonly List<string> _sourceFiles;
    private readonly List<string> _includeDirs;

    private void LogFiles()
    {
        Console.WriteLine("Header files:");
        foreach (var file in _headerFiles)
        {
            Console.WriteLine($"\t{file}");
        }

        Console.WriteLine("Source files:");
        foreach (var file in _sourceFiles)
        {
            Console.WriteLine($"\t{file}");
        }
    }

    public GameNetworkingSocketsLibrary(string repoPath, string buildPath, string outputPath)
    {
        _outputPath = outputPath;

        _includePath = Path.Combine(repoPath, "include");
        _steamIncludePath = Path.Combine(_includePath, "steam");
        _sourcePath = Path.Combine(repoPath, "src");
        _libraryPath = Path.Combine(buildPath, "bin");

        _includeDirs = new List<string>
        {
            _includePath,
            // _steamIncludePath
            // Path.Combine(_sourcePath, "public"),
            // Path.Combine(_sourcePath, "common"),
            // Path.Combine(buildPath, "src")
        };

        _headerFiles = new List<string>
        {
            Path.Combine(_steamIncludePath, "steamnetworkingsockets.h"),
            Path.Combine(_steamIncludePath, "steamnetworkingsockets_flat.h"),
            // Path.Combine(_steamIncludePath, "isteamnetworkingmessages.h"),
            // Path.Combine(_steamIncludePath, "isteamnetworkingutils.h"),
            // Path.Combine(_steamIncludePath, "steamnetworkingcustomsignaling.h"),
            // Path.Combine(_steamIncludePath, "steamnetworkingtypes.h"),
            // Path.Combine(_steamIncludePath, "steamtypes.h"),
            // Path.Combine(_steamIncludePath, "steamuniverse.h"),
            // Path.Combine(_steamIncludePath, "steamclientpublic.h")
        };

        _sourceFiles = new List<string>();
        // _sourceFiles.AddRange(
        //     Directory.GetFiles(Path.Combine(_sourcePath, "steamnetworkingsockets"), "*.cpp")
        // );
        // _sourceFiles.AddRange(
        //     Directory.GetFiles(Path.Combine(_sourcePath, "steamnetworkingsockets", "clientlib"), "*.cpp")
        // );

        // _headerFiles.AddRange(
        //     Directory.GetFiles(Path.Combine(_sourcePath, "steamnetworkingsockets"), "*.h")
        // );
        // _headerFiles.AddRange(
        //     Directory.GetFiles(Path.Combine(_sourcePath, "steamnetworkingsockets", "clientlib"), "*.h")
        // );

        LogFiles();
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
        driver.Options.Verbose = true;

        var module = driver.Options.AddModule(LibraryName);
        module.OutputNamespace = DotNetLibraryName;

        module.IncludeDirs.AddRange(_includeDirs);
        module.Headers.AddRange(_headerFiles);

        module.LibraryDirs.Add(_libraryPath);
        module.Libraries.Add($"lib{LibraryName}.so");

        module.CodeFiles.AddRange(_sourceFiles);
    }

    public void SetupPasses(Driver driver)
    {

    }

    public void Preprocess(Driver driver, ASTContext ctx)
    {
        // ctx.IgnoreClassWithName("ISteamNetworkingFakeUDPPort");
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
            if (output.TranslationUnit.FileName == "Std.h")
            {
                output.Outputs.Clear();
                break;
            }
        }
    }
}