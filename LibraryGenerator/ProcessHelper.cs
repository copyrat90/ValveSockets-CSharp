using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace LibraryGenerator
{
    static class ProcessHelper
    {
        internal static bool InstallGlobalDotnetTool(string toolName)
        {
            Console.WriteLine($"Installing dotnet tool globally: {toolName}");

            using Process dotnetProcess = new();
            dotnetProcess.StartInfo.FileName = "dotnet";
            dotnetProcess.StartInfo.Arguments = $"tool install -g {toolName}";
            dotnetProcess.StartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            dotnetProcess.StartInfo.Environment.Add("LC_ALL", "C");
            dotnetProcess.StartInfo.RedirectStandardError = true;

            dotnetProcess.Start();
            dotnetProcess.WaitForExit();

            if (dotnetProcess.ExitCode == 1 && dotnetProcess.StandardError.ReadLine()!.Contains("already installed"))
            {
                return true;
            }

            return dotnetProcess.ExitCode == 0;
        }

        internal static bool CloneGitRepo(string url, string workDir, bool recursive = false, string extraOptions = "")
        {
            int nameStartIndex = url.LastIndexOf("/") + 1;
            string repoName = url.Substring(nameStartIndex,
                url.EndsWith(".git") ? url.Length - (nameStartIndex + 4) : url.Length - nameStartIndex);

            if (Directory.Exists(Path.Combine(workDir, repoName)))
            {
                Console.WriteLine($"{repoName} already exists.");

                return true;
            }

            Console.WriteLine($"Cloning {repoName}...");

            using Process gitProcess = new();
            gitProcess.StartInfo.FileName = "git";
            gitProcess.StartInfo.Arguments =
                recursive ? $"clone {url} --recurse-submodules {extraOptions}" : $"clone {url} {extraOptions}";
            gitProcess.StartInfo.WorkingDirectory = workDir;

            gitProcess.Start();
            gitProcess.WaitForExit();

            if (gitProcess.ExitCode != 0)
            {
                Console.WriteLine($"\nCouldn't clone repository. (Exit code: {gitProcess.ExitCode})");
            }

            return gitProcess.ExitCode == 0;
        }

        internal static bool InvokeCmake(string path, string buildPath, string extraOptions = "")
        {
            if (Directory.Exists(buildPath))
            {
                Console.WriteLine("CMake project already generated.");

                return true;
            }

            Directory.CreateDirectory(buildPath);

            Console.WriteLine("Generating CMake project...");

            using Process cmakeProcess = new();
            cmakeProcess.StartInfo.FileName = "cmake";
            cmakeProcess.StartInfo.Arguments = $"{extraOptions} {path}";
            cmakeProcess.StartInfo.WorkingDirectory = buildPath;

            cmakeProcess.Start();
            cmakeProcess.WaitForExit();

            return cmakeProcess.ExitCode == 0;
        }

        internal static bool BuildMakeProject(string path, string extraOptions = "")
        {
            using Process makeProcess = new();
            makeProcess.StartInfo.FileName = "make";
            makeProcess.StartInfo.Arguments = $"-j{Environment.ProcessorCount} {extraOptions}";
            makeProcess.StartInfo.WorkingDirectory = path;

            makeProcess.Start();
            makeProcess.WaitForExit();

            return makeProcess.ExitCode == 0;
        }

        internal static bool BuildNinjaProject(string path, string extraOptions = "")
        {
            using Process makeProcess = new();
            makeProcess.StartInfo.FileName = "ninja";
            makeProcess.StartInfo.Arguments = extraOptions;
            makeProcess.StartInfo.WorkingDirectory = path;

            makeProcess.Start();
            makeProcess.WaitForExit();

            return makeProcess.ExitCode == 0;
        }

        internal static int InvokeWithEnvVars(string filename, string arguments, string workDir,
            Dictionary<string, string> environment)
        {
            using Process process = new();
            process.StartInfo.FileName = filename;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.WorkingDirectory = workDir;

            foreach (var variable in environment)
            {
                process.StartInfo.Environment.Add(variable);
            }

            process.Start();
            process.WaitForExit();

            return process.ExitCode;
        }
    }
}