using System.Diagnostics;
using System.Reflection;
using CppSharp;

namespace LibraryGenerator;

public static class Program
{
    private const string Repo = "GameNetworkingSockets";
    private const string RepoUrl = $"https://github.com/ValveSoftware/{Repo}";
    private const string BuildingUrl = $"{RepoUrl}/blob/master/BUILDING.md";
    private const string OutputName = "Valve.Sockets";
    public static int Main(string[] args)
    {
        string outputDir = Path.Join(Path.GetFullPath(args.Length > 0 ? args[0] : Directory.GetCurrentDirectory()), OutputName);
        string programDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        string repoDir = Path.Join(programDir, Repo);
        string buildDir = Path.Join(repoDir, "build");

        Console.WriteLine("Make sure git and the required packages are installed.");
        Console.WriteLine($"You can read the build guide here: {BuildingUrl}");

        Console.WriteLine("\nOptions:");
        Console.WriteLine($"\tRepository path: {repoDir}");
        Console.WriteLine($"\tBuild path: {buildDir}");
        Console.WriteLine($"\tOutput path: {outputDir}");

        Thread.Sleep(2000);
        Console.WriteLine("\nPreparing...");

        bool repoExists = Directory.Exists(repoDir);

        if (repoExists && Directory.Exists(buildDir))
        {
            Console.WriteLine("Existing repository found!");
            Directory.Delete(buildDir, true);
        }

        if (!repoExists)
        {
            Console.WriteLine($"Cloning {Repo}...");

            using Process gitProcess = new();
            gitProcess.StartInfo.FileName = "git";
            gitProcess.StartInfo.Arguments = $"clone {RepoUrl}";
            gitProcess.StartInfo.WorkingDirectory = programDir;
            gitProcess.Start();
            gitProcess.WaitForExit();

            if (gitProcess.ExitCode != 0)
            {
                Console.WriteLine($"\nCouldn't clone repository. (Exit code: {gitProcess.ExitCode})");
                return 1;
            }
        }

        Console.WriteLine("Creating build directory...");
        Directory.CreateDirectory(buildDir);

        Console.WriteLine("Generating project...");

        using Process cmakeProcess = new();
        cmakeProcess.StartInfo.FileName = "cmake";
        cmakeProcess.StartInfo.Arguments = "-G Ninja ..";
        cmakeProcess.StartInfo.WorkingDirectory = buildDir;
        cmakeProcess.Start();
        cmakeProcess.WaitForExit();

        Console.WriteLine("\nBuilding project...");

        using Process ninjaProcess = new();
        ninjaProcess.StartInfo.FileName = "ninja";
        ninjaProcess.StartInfo.WorkingDirectory = buildDir;
        ninjaProcess.Start();
        ninjaProcess.WaitForExit();

        Console.WriteLine("\nDone!");

        if (Directory.Exists(outputDir))
        {
            Console.WriteLine($"Cleaning existing output path: {outputDir}");
            Directory.Delete(outputDir, true);
        }

        Directory.CreateDirectory(outputDir);

        Console.WriteLine("\nGenerating bindings...");

        ConsoleDriver.Run(new GameNetworkingSocketsLibrary(repoDir, buildDir, outputDir));

        return 0;
    }
}
