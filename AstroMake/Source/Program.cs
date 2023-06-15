using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Linq;

namespace AstroMake;

internal static class Program
{
    private static int Main(String[] Arguments)
    {
        // Hello Astro Make
        Log.Trace($"Astro Make {Version.AstroVersion}");
        Log.Trace("Copyright (C) 2023 Erwann Messoah");
        Log.Trace("Using dotnet framework 4.8.1\n");
        
        // Show help if no arguments
        if (Arguments.Length < 1)
        {
            ShowHelp();
            return Error.NoArgumentsProvided.ToInt();
        }

        // Help arguments
        if (Arguments.Length is 1)
        {
            if (Arguments[0] == "/h" || Arguments[0] == "/help")
            {
                ShowHelp();
            }
            else
            {
                Log.Error("Error: Bad arguments usage.");
                ShowHelp();
                return Error.BadArgumentsUsage.ToInt();
            }
            return Error.NoError.ToInt();
        }

        if (Arguments.Length is 2)
        {
            if (Arguments[0] == "/b" || Arguments[0] == "/build")
            {
                return Generate().ToInt();
            }
            else
            {
                Log.Error("Error: Bad arguments usage.");
                ShowHelp();
                return Error.BadArgumentsUsage.ToInt();
            }
        }
        else
        {
            Log.Error("Error: Bad arguments usage.");
            ShowHelp();
            return Error.BadArgumentsUsage.ToInt();
        }
    }
    
    private static void ShowHelp()
    {
        Log.Trace("Usage: AstroMake [options]");
        Log.Trace("options:");
        Log.Trace("    /h, /help                       Show help");
        Log.Trace("    /b, /build [target]             Generate using AstroMake scripts");
        Log.Trace("targets:");
        Log.Trace("    vstudio     Generate Visual Studio Solution (latest version)");
        Log.Trace("    makefile    Generate Makefiles");
        Log.Trace("    xcode       Generate XCode Project");
    }

    private static Error Generate()
    {
        String CurrentDirectory = Directory.GetCurrentDirectory();
        Log.Trace($"Current Working Directory: {CurrentDirectory}");
        List<String> BuildFilepaths = Directory.EnumerateFiles(CurrentDirectory, "*.Astro.cs", SearchOption.AllDirectories).ToList();
        if (BuildFilepaths.Count <= 0)
        {
            Log.Error("No build scripts found!");
            return Error.NoBuildScriptFound;
        }
        
        Log.Trace($"Found {BuildFilepaths.Count} build script(s):");
        foreach (var BuildFilepath in BuildFilepaths)
        {
            Log.Trace($"--> {BuildFilepath}");
        }
        
        Log.Trace("Compiling scripts...");
        Stopwatch Stopwatch = new Stopwatch();
        Stopwatch.Start();
        using CSharpCodeProvider CodeProvider = new CSharpCodeProvider();
        CompilerParameters Parameters = new CompilerParameters
        {
            GenerateInMemory = true,
            GenerateExecutable = false,
            IncludeDebugInformation = true,
            OutputAssembly = "AstroMakeGenerator",
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
            return Error.CompileError;
        }
        Stopwatch.Stop();
        Log.Success($"Compilation successful! Took {Stopwatch.ElapsedMilliseconds / 1000.0f}s.");
        
        
        
        Stopwatch.Reset();
        Stopwatch.Start();
        Log.Trace("Generating Visual Studio Solution...");
        Assembly CompiledAssembly = CompileResults.CompiledAssembly;

        // Current workspace is the first workspace that as a BuildAttribute
        Workspace Workspace = Activator.CreateInstance(CompiledAssembly.GetTypes().Single(Type =>
            Type.IsSubclassOf(typeof(Workspace)) &&
            Type.GetCustomAttribute(typeof(BuildAttribute)) != null)) as Workspace;

        
        IEnumerable<Type> ApplicationsTypes = CompiledAssembly.GetTypes().Where(Type =>
            Type.IsSubclassOf(typeof(Application)) &&
            Type.GetCustomAttribute(typeof(BuildAttribute)) != null);
        
        

        List<Application> Applications = new List<Application>();
        foreach (Type Type in ApplicationsTypes)
        {
            Applications.Add(Activator.CreateInstance(Type, Workspace) as Application);
        }
        
        
        
        foreach (var App in Applications)
        {
            String Filepath = Path.Combine(CurrentDirectory, $"test{Extensions.VisualCXXProject}");
            using FileStream Stream = File.Open(Filepath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using VcxprojWriter Writer = new(Stream);
            Writer.WriteApplication(Workspace, App);
        }
        
        
        
        Log.Success($"Visual Studio Solution Generation sucessful! Took {Stopwatch.ElapsedMilliseconds / 1000.0f}s");
        return Error.NoError;
    }
}