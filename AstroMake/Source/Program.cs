﻿using System;
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
    private static void Main(String[] Arguments)
    {
        // Hello Astro Make
        Log.Trace($"Astro Make {Version.AstroVersion}");
        Log.Trace("Copyright (C) 2023 Erwann Messoah");
        Log.Trace("Using dotnet framework 4.8.1\n");

        // Parse the arguments
        try
        {
            ArgumentParser<Options> Parser = new ArgumentParser<Options>(Arguments, ArgumentParserSettings.Default);
            Parser.Parse(options =>
            {
                if (options.Help)
                {
                    ShowHelp();
                }

                if (options.Type is Options.BuildType.None)
                {
                    Generate();
                }
            });
        }
        catch (BadArgumentUsageException Exception)
        {
            Log.Error(Exception.Error.ToStr());
            ShowHelp();
            Environment.Exit((int)Error.BadArgumentsUsage);
        }
        catch (NoArgumentProvidedException Exception)
        {
            Log.Error(Exception.Error.ToStr());
            ShowHelp();
            Environment.Exit((int)Error.NoArgumentsProvided);
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
            String Filepath = null;
            ApplicationWriter Writer = null;
            if (App.Language == Language.C || App.Language == Language.CPlusPlus)
            {
                Filepath = Path.Combine(CurrentDirectory, $"test{Extensions.VisualCXXProject}");
                using FileStream Stream = File.Open(Filepath, FileMode.OpenOrCreate, FileAccess.Write);
                Writer = new VcxprojWriter(Stream);
            }

            if (App.Language == Language.CSharp)
            {
                Filepath = Path.Combine(CurrentDirectory, $"test{Extensions.VisualCSharpProject}");
                using FileStream Stream = File.Open(Filepath, FileMode.OpenOrCreate, FileAccess.Write);
                Writer = new CsprojWriter(Stream);
            }
           
            Writer?.Write(Workspace, App);
            Log.Success($"Generated {Filepath}!");
        }
        
        
        
        Log.Success($"Visual Studio Solution Generation sucessful! Took {Stopwatch.ElapsedMilliseconds / 1000.0f}s");
        return Error.NoError;
    }
}

