using LibraryGenerator.CppSharp;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace LibraryGenerator;

public static class Program
{
    private const string PInvokeGenerator = "ClangSharpPInvokeGenerator";
    private const string LlvmRepo = "llvm-project";
    private const string LlvmRepoUrl = $"https://github.com/llvm/{LlvmRepo}";
    private const string LlvmBranch = "llvmorg-15.0.0";
    private const string ClangSharpRepo = "clangsharp";
    private const string ClangSharpRepoUrl = $"https://github.com/dotnet/{ClangSharpRepo}";
    private const string GNSRepo = "GameNetworkingSockets";
    private const string GNSRepoUrl = $"https://github.com/ValveSoftware/{GNSRepo}";
    private const string GNSBuildingUrl = $"{GNSRepoUrl}/blob/master/BUILDING.md";

    private const string OutputName = "Valve.Sockets";

    private static string GetPInvokeGeneratorArgs(string gnsDir, string outputDir)
    {
        string gccVersionPath = Directory.GetDirectories("/usr/lib/gcc/x86_64-pc-linux-gnu").Max();
        string gnsIncludeSteamDir = Path.Combine(gnsDir, "include", "steam");
        StringBuilder builder = new();

        // Configuration options
        builder.AppendLine("--config");
        builder.AppendLine("log-exclusions");
        builder.AppendLine("log-potential-typedef-remappings");
        builder.AppendLine("multi-file");
        builder.AppendLine("compatible-codegen");
        builder.AppendLine("unix-types");

        // Include directories
        builder.AppendLine("--include-directory");
        builder.AppendLine(Path.Combine(gnsDir, "include"));
        builder.AppendLine(Path.Combine(gccVersionPath, "include"));

        // Header files
        builder.AppendLine("--file");
        builder.AppendLine(Path.Combine(gnsIncludeSteamDir, "isteamnetworkingutils.h"));
        builder.AppendLine(Path.Combine(gnsIncludeSteamDir, "isteamnetworkingsockets.h"));
        builder.AppendLine(Path.Combine(gnsIncludeSteamDir, "isteamnetworkingmessages.h"));
        builder.AppendLine(Path.Combine(gnsIncludeSteamDir, "steamclientpublic.h"));
        builder.AppendLine(Path.Combine(gnsIncludeSteamDir, "steamnetworkingsockets.h"));
        builder.AppendLine(Path.Combine(gnsIncludeSteamDir, "steamnetworkingsockets_flat.h"));
        builder.AppendLine(Path.Combine(gnsIncludeSteamDir, "steamnetworkingtypes.h"));
        builder.AppendLine(Path.Combine(gnsIncludeSteamDir, "steamtypes.h"));
        builder.AppendLine(Path.Combine(gnsIncludeSteamDir, "steamuniverse.h"));

        // Generated namespace
        builder.AppendLine("--namespace");
        builder.AppendLine(OutputName);

        // Generated PInvoke filename
        builder.AppendLine("--methodClassName");
        builder.AppendLine("Native");

        // Native library name
        builder.AppendLine("--libraryPath");
        builder.AppendLine(GNSRepo);

        // Output path
        builder.AppendLine("--output");
        builder.AppendLine(outputDir);

        return builder.Replace("\n", " ").ToString();
    }

    public static int Main(string[] args)
    {
        string workDir = Path.Combine(Path.GetFullPath(args.Length > 0 ? args[0] : Directory.GetCurrentDirectory()), "gen");
        string outputDir = Path.Combine(Path.GetFullPath(args.Length > 1 ? args[1] : Directory.GetCurrentDirectory()), OutputName);

        Console.WriteLine("Make sure git and the required packages are installed.");
        Console.WriteLine($"You can read the build guide here: {GNSBuildingUrl}");

        Console.WriteLine("\nOptions:");
        Console.WriteLine($"\tWork directory: {workDir}");
        Console.WriteLine($"\tOutput directory: {outputDir}");

        if (!Directory.Exists(workDir))
        {
            Directory.CreateDirectory(workDir);
        }

        // Phase 1: Install dotnet tool

        if (!ProcessHelper.InstallGlobalDotnetTool(PInvokeGenerator))
        {
            Console.WriteLine($"Failed to install dotnet tool {PInvokeGenerator} globally. Aborting.");

            return 1;
        }

        // Phase 2: Clone repos

        Console.WriteLine("\nCloning repos...");

        if (!ProcessHelper.CloneGitRepo(LlvmRepoUrl, workDir, extraOptions: $"--single-branch --branch {LlvmBranch}"))
        {
            Console.WriteLine($"Failed to clone {LlvmRepo}. Aborting.");

            return 1;
        }

        if (!ProcessHelper.CloneGitRepo(ClangSharpRepoUrl, workDir))
        {
            Console.WriteLine($"Failed to clone {ClangSharpRepo}. Aborting.");

            return 1;
        }

        if (!ProcessHelper.CloneGitRepo(GNSRepoUrl, workDir, true))
        {
            Console.WriteLine($"Failed to clone {GNSRepo}. Aborting.");

            return 1;
        }

        // Phase 3: Build GameNetworkingSockets

        Console.WriteLine($"\nBuilding {GNSRepo}...");

        string gnsBuildDir = Path.Join(workDir, GNSRepo, "build");

        if (!ProcessHelper.InvokeCmake(Path.Combine(workDir, GNSRepo), gnsBuildDir, "-G Ninja -DUSE_STEAMWEBRTC=ON"))
        {
            Console.WriteLine($"Couldn't generate CMake project for {GNSRepo}. Aborting.");

            return 1;
        }

        if (!File.Exists(Path.Combine(gnsBuildDir, "bin", $"lib{GNSRepo}.so")))
        {
            if (!ProcessHelper.BuildNinjaProject(gnsBuildDir))
            {
                Console.WriteLine($"Failed to build {GNSRepo}. Aborting.");

                return 1;
            }
        }
        else
        {
            Console.WriteLine("Existing library found. Skipping build.");
        }

        // Phase 4: Build dependencies for libClangSharp

        Console.WriteLine("\nBuilding dependencies...");

        string clangBuildPath = Path.Combine(workDir, LlvmRepo, "artifacts", "bin");
        string clangInstallPath = Path.Combine(workDir, LlvmRepo, "artifacts", "install");

        bool cmakeResult = ProcessHelper.InvokeCmake(
            Path.Combine(workDir, LlvmRepo, "llvm"),
            clangBuildPath,
            "-DLLVM_ENABLE_PROJECTS=clang -DCMAKE_INSTALL_PREFIX=../install -DCMAKE_BUILD_TYPE=Release -G \"Unix Makefiles\""
        );

        if (!cmakeResult)
        {
            Console.WriteLine($"Couldn't generate CMake project for {LlvmRepo}/{LlvmBranch}. Aborting.");

            return 1;
        }

        if (!File.Exists(Path.Combine(clangInstallPath, "lib", "libclang.so")))
        {
            Console.WriteLine("Building clang...");

            if (!ProcessHelper.BuildMakeProject(clangBuildPath))
            {
                Console.WriteLine("Failed to build clang. Aborting.");

                return 1;
            }

            Console.WriteLine("Installing clang...");

            if (!ProcessHelper.BuildMakeProject(clangBuildPath, "install"))
            {
                Console.WriteLine("Failed to install clang. Aborting.");
            }
        }
        else
        {
            Console.WriteLine("Clang already built and installed.");
        }

        // Phase 5: Build libClangSharp

        string clangSharpBuildPath = Path.Combine(workDir, ClangSharpRepo, "artifacts", "bin", "native");

        cmakeResult = ProcessHelper.InvokeCmake(
            Path.Combine(workDir, ClangSharpRepo),
            clangSharpBuildPath,
            $"-DPATH_TO_LLVM=\"{clangInstallPath}\""
        );

        if (!cmakeResult)
        {
            Console.WriteLine($"Couldn't generate CMake project for {ClangSharpRepo}. Aborting.");

            return 1;
        }

        if (!ProcessHelper.BuildMakeProject(clangSharpBuildPath))
        {
            Console.WriteLine("Failed to build libClangSharp. Aborting.");

            return 1;
        }

        // Phase 6: Invoke PInvokeGenerator

        Console.WriteLine($"\nPreparing {PInvokeGenerator}...");

        string clangSharpLibPath = Path.Combine(clangSharpBuildPath, "lib");
        string clangLibPath = Path.Combine(clangInstallPath, "lib");

        if (Directory.Exists(outputDir))
        {
            Console.WriteLine($"Cleaning existing output directory: {outputDir}");
            foreach (var file in Directory.EnumerateFiles(outputDir, "*.cs", SearchOption.AllDirectories))
            {
                #if DEBUG
                if (Path.GetFileName(file) == "NativeTypeNameAttribute.cs")
                {
                    continue;
                }
                #endif

                File.Delete(file);
            }
        }

        Directory.CreateDirectory(outputDir);

        #if RELEASE
        File.Copy(Path.Combine(Directory.GetCurrentDirectory(), "NativeTypeNameAttribute.cs"), Path.Combine(outputDir, "NativeTypeNameAttribute.cs"));
        #endif

        Console.WriteLine($"[ClangSharp] Generating {GNSRepo} bindings...");

        int generatorResult = ProcessHelper.InvokeWithEnvVars(
            PInvokeGenerator,
            GetPInvokeGeneratorArgs(Path.Combine(workDir, GNSRepo), outputDir),
            workDir, new()
            {
                {"LD_LIBRARY_PATH", $"{clangSharpLibPath}:{clangLibPath}"}
            }
        );

        if (generatorResult != 0 && generatorResult != 2)
        {
            Console.WriteLine($"{PInvokeGenerator} couldn't generate {GNSRepo} bindings. Aborting.");

            return 1;
        }

        Console.WriteLine("Success!\n");

        // Phase 7: Run CppSharp source generator

        Console.WriteLine($"[CppSharp] Generating {GNSRepo} bindings...");

        ConsoleLibraryDriver.Run(new GameNetworkingSocketsLibrary(Path.Combine(workDir, GNSRepo), outputDir));

        File.Delete(Path.Combine(outputDir, $"{OutputName}-symbols.cpp"));

        foreach (string filePath in Directory.EnumerateFiles(outputDir, "i*.cs"))
        {
            Span<char> filename = Path.GetFileName(filePath).ToCharArray();

            filename[0] = char.ToUpper(filename[0]);
            filename[1] = char.ToUpper(filename[1]);
            filename[6] = char.ToUpper(filename[6]);
            filename[16] = char.ToUpper(filename[16]);

            string newFilePath = Path.Combine(outputDir, new string(filename));

            File.Delete(newFilePath);
            File.Move(filePath, newFilePath);
        }

        Console.WriteLine("Success!\n");

        // Phase 8: Clean up generated files using CSharpAnalyzer

        Console.WriteLine("Initializing CSharpAnalyzer...");

        CSharpAnalyzer analyzer = new(outputDir);

        Console.WriteLine("Cleaning generated files...");

        analyzer.FixFiles();

        Console.WriteLine("Done!");

        return 0;
    }
}