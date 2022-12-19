using System.Diagnostics;
using CppSharp;
using CppSharp.AST;
using CppSharp.Passes;
using System.Linq;

namespace LibraryGenerator.CppSharp;

public static class ConsoleLibraryDriver
{
    public static void Run(ILibrary library)
    {
        var options = new DriverOptions();
        using var driver = new Driver(options);

        library.Setup(driver);

        driver.Setup();

        if (driver.Options.IsCSharpGenerator)
        {
            var generatorProperty = driver.GetType().GetProperty("Generator");
            var generator = new CSharpLibraryGenerator(driver.Context);

            Diagnostics.Message("Replacing CSharpGenerator with CSharpLibraryGenerator...");

            Debug.Assert(generatorProperty != null, $"{nameof(generatorProperty)} != null");
            generatorProperty.SetValue(driver, generator);
        }

        if (driver.Options.Verbose)
            Diagnostics.Level = DiagnosticKind.Debug;

        if (!options.Quiet)
            Diagnostics.Message("Parsing libraries...");

        if (!driver.ParseLibraries())
            return;

        if (!options.Quiet)
            Diagnostics.Message("Parsing code...");

        if (!driver.ParseCode())
        {
            Diagnostics.Error("CppSharp has encountered an error while parsing code.");
            return;
        }

        new CleanUnitPass { Context = driver.Context }.VisitASTContext(driver.Context.ASTContext);
        options.Modules.RemoveAll(m => m != options.SystemModule && !m.Units.GetGenerated().Any());

        if (!options.Quiet)
            Diagnostics.Message("Processing code...");

        driver.SetupPasses(library);
        driver.SetupTypeMaps();
        driver.SetupDeclMaps();

        library.Preprocess(driver, driver.Context.ASTContext);

        driver.ProcessCode();
        library.Postprocess(driver, driver.Context.ASTContext);

        if (!options.Quiet)
            Diagnostics.Message("Generating code...");

        if (!options.DryRun)
        {
            var outputs = driver.GenerateCode();

            library.GenerateCode(driver, outputs);

            foreach (var output in outputs)
            {
                foreach (var pass in driver.Context.GeneratorOutputPasses.Passes)
                {
                    pass.VisitGeneratorOutput(output);
                }
            }

            driver.SaveCode(outputs);
            if (driver.Options.IsCSharpGenerator && driver.Options.CompileCode)
                driver.Options.Modules.Any(m => !driver.CompileCode(m));
        }
    }
}