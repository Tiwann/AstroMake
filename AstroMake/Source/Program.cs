using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Linq;


namespace AstroMake;

public static class Options
{
    public static readonly CommandLineOption Help = new('h', "help", false, false, "Show help");
    public static readonly CommandLineOption Build = new('b', "build", true, false, "Generate using Astro Make build scripts",
            new("targets",
                new("vstudio",  "Generate Visual Studio Solution (latest version)"),
                new("makefile", "Generate Makefiles"),
                new("xcode",    "Generate XCode Project")));
    
    public static readonly CommandLineOption RootDir = new('d', "dir", false, false, "Specify a build script search root directory");
    public static readonly CommandLineOption Source = new('s', "source", false, true, "Add specific build script to the build queue");
}


internal static class Program
{
    public static string ScriptSearchRootDirectory = Directory.GetCurrentDirectory();
    
    private static void Main(string[] Arguments)
    {
        // Hello Asro Make
        Log.Trace($"Astro Make {Version.AstroVersion}");
        Log.Trace("Copyright (C) 2023 Erwann Messoah");
        Log.Trace("Using dotnet framework 4.8.1\n");

        //Setting up the parser
        ArgumentParser Parser = new(Arguments, ArgumentParserSettings.WindowsStyle);
        Parser.AddOptions(Options.Help, Options.Source, Options.Build, Options.RootDir);
        
        // Parser the arguments
        try
        {
            Parser.Parse();
            if (Parser.GetBool(Options.Help))
            {
                Log.Trace(Parser.GetHelpText());
            }

            var a = Parser.GetString(Options.Source);

        }
        catch (InvalidCommandLineArgumentException Exception)
        {
            Log.Error(Exception.Message);
            Log.Trace(Parser.GetHelpText());
            Environment.Exit(0);
        }
        catch (ArgumentException Exception)
        {
            Log.Error(Exception.Message);
            Environment.Exit(0);
        }
        catch (NoArgumentProvidedException Exception)
        {
            Log.Error(Exception.Message);
            Environment.Exit(0);
        }
    }
    
    //TODO: Rewrite this Generate method, should write a BuildQueue class that set up things to write vcxproj and sln files
    private static void Generate()
    {
        string CurrentDirectory = ScriptSearchRootDirectory;
        Log.Trace($"Current Working Directory: {CurrentDirectory}");
        List<string> BuildFilepaths = Directory.EnumerateFiles(CurrentDirectory, "*.Astro.cs", SearchOption.AllDirectories).ToList();
        if (BuildFilepaths.Count <= 0)
        {
            Log.Error("No build scripts found!");
            return;
        }
        
        Log.Trace($"Found {BuildFilepaths.Count} build script(s):");
        foreach (var BuildFilepath in BuildFilepaths)
        {
            Log.Trace($"--> {BuildFilepath}");
        }
        
        Log.Trace("Compiling scripts...");
        Stopwatch Stopwatch = new();
        Stopwatch.Start();
        using CSharpCodeProvider CodeProvider = new();
        CompilerParameters Parameters = new()
        {
            GenerateInMemory = true,
            GenerateExecutable = false,
            IncludeDebugInformation = true,
            ReferencedAssemblies = { Assembly.GetExecutingAssembly().Location },
        };
        
        CompilerResults CompileResults = CodeProvider.CompileAssemblyFromFile(Parameters, BuildFilepaths.ToArray());
        
        if (CompileResults.Errors.HasErrors)
        {
            foreach (CompilerError CompileError in CompileResults.Errors)
            {
                Log.Error($"{CompileError.ErrorText}.");
                Log.Error($"File:{CompileError.FileName}. Line: {CompileError.Line} Col: {CompileError.Column}");
            }

            return;
        }
        Stopwatch.Stop();
        Log.Success($"Compilation successful! Took {Stopwatch.ElapsedMilliseconds / 1000.0f}s.");
        
        
        
        Stopwatch.Reset();
        Stopwatch.Start();
        Log.Trace("Generating Visual Studio Solution...");
        Assembly CompiledAssembly = CompileResults.CompiledAssembly;

        // Current workspace is the first workspace that as a BuildAttribute
        Solution Solution = Activator.CreateInstance(CompiledAssembly.GetTypes().Single(Type =>
            Type.IsSubclassOf(typeof(Solution)) &&
            Type.GetCustomAttribute(typeof(BuildAttribute)) != null)) as Solution;

        
        IEnumerable<Type> ApplicationsTypes = CompiledAssembly.GetTypes().Where(Type =>
            Type.IsSubclassOf(typeof(Application)) &&
            Type.GetCustomAttribute(typeof(BuildAttribute)) != null);
        
        

        List<Application> Applications = new List<Application>();
        foreach (Type Type in ApplicationsTypes)
        {
            Applications.Add(Activator.CreateInstance(Type, Solution) as Application);
        }
        
        
        
        foreach (var App in Applications)
        {
            string Filepath = Path.Combine(CurrentDirectory, $"test{Extensions.VisualCXXProject}");
            using FileStream Stream = File.Open(Filepath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using VcxprojWriter Writer = new(Stream);     
            Writer.Write(Solution, App);
            Log.Success($"Generated {Filepath}!");
        }
        
        Log.Success($"Visual Studio Solution Generation sucessful! Took {Stopwatch.ElapsedMilliseconds / 1000.0f}s");
    }
}

