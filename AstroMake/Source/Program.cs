﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;


namespace AstroMake;


internal static class Program
{
    public static String ScriptSearchRootDirectory = Directory.GetCurrentDirectory();
    
    private static void Main(String[] Arguments)
    {
        // Hello Asro Make
        Log.Trace($"Astro Make {Version.AstroVersion}");
        Log.Trace("Copyright (C) 2023 Erwann Messoah");
        Log.Trace("Using dotnet framework 4.8.1\n");

        //Setting up the parser
        ArgumentParser Parser = new(Arguments, ArgumentParserSettings.WindowsStyle);
        Parser.AddOptions(new Collection<CommandLineOption>
        {
            new(ShortName: 'h', LongName: "help",   Required: false, AllowMultiple: false),
            new(ShortName: 'b', LongName: "build",  Required: true,  AllowMultiple: false),
            new(ShortName: 's', LongName: "source", Required: false, AllowMultiple: true),
        });
        
        // Parser the arguments
        try
        {
            Parser.Parse();
        }
        catch (InvalidCommandLineArgumentException Exception)
        {
            Log.Error($"{Exception.Message}");
            ShowHelp();
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

    private static void Generate()
    {
        String CurrentDirectory = ScriptSearchRootDirectory;
        Log.Trace($"Current Working Directory: {CurrentDirectory}");
        List<String> BuildFilepaths = Directory.EnumerateFiles(CurrentDirectory, "*.Astro.cs", SearchOption.AllDirectories).ToList();
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
            String Filepath = Path.Combine(CurrentDirectory, $"test{Extensions.VisualCXXProject}");
            using FileStream Stream = File.Open(Filepath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using VcxprojWriter Writer = new(Stream);     
            Writer.Write(Solution, App);
            Log.Success($"Generated {Filepath}!");
        }
        
        
        
        Log.Success($"Visual Studio Solution Generation sucessful! Took {Stopwatch.ElapsedMilliseconds / 1000.0f}s");
    }
}

