using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;

namespace LibraryGenerator;

public class GameNetworkingSocketsLibrary : ILibrary
{
    private readonly string _repoPath;
    private readonly string _buildPath;
    private readonly string _outputPath;

    public GameNetworkingSocketsLibrary(string repoPath, string buildPath, string outputPath)
    {
        _repoPath = repoPath;
        _buildPath = buildPath;
        _outputPath = outputPath;
    }

    public void Setup(Driver driver)
    {
        if (!OperatingSystem.IsWindows())
        {
            driver.ParserOptions.MicrosoftMode = false;
            driver.ParserOptions.TargetTriple = "x86_64-linux-gnu";
        }
        else
        {
            driver.ParserOptions.MicrosoftMode = true;
        }

        driver.Options.GeneratorKind = GeneratorKind.CSharp;
        driver.Options.UseHeaderDirectories = true;
        driver.Options.GenerationOutputMode = GenerationOutputMode.FilePerUnit;
        driver.Options.CommentKind = CommentKind.BCPLSlash;
        driver.Options.OutputDir = _outputPath;
        driver.Options.Verbose = true;

        var module = driver.Options.AddModule("Valve.Sockets");

        module.IncludeDirs.Add(Path.Combine(_repoPath, "include", "steam"));

        module.Headers.Add(Path.Combine(_repoPath, "include", "steam", "isteamnetworkingsockets.h"));
        module.Headers.Add(Path.Combine(_repoPath, "include", "steam", "steamnetworkingtypes.h"));

        module.LibraryDirs.Add(Path.Combine(_buildPath, "bin"));
        module.Libraries.Add("libGameNetworkingSockets.so");

        module.OutputNamespace = "Valve.Sockets";
    }

    public void SetupPasses(Driver driver)
    {
        Console.WriteLine("SetupPasses called.");
    }

    public void Preprocess(Driver driver, ASTContext ctx)
    {
        Console.WriteLine("Preprocess called.");
    }

    public void Postprocess(Driver driver, ASTContext ctx)
    {
        Console.WriteLine("Postprocess called.");
    }
}
