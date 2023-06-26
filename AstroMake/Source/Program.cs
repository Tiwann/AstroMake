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
    private static void Main(string[] Arguments)
    {
        // Hello Asro Make
        Log.Trace($"Astro Make {Version.AstroVersion}");
        Log.Trace("Copyright (C) 2023 Erwann Messoah");
        Log.Trace("Using dotnet framework 4.8.1\n");

        // Setting up the parser
        ArgumentParser Parser = new(Arguments, ArgumentParserSettings.WindowsStyle);
        Parser.AddOptions(Options.Help, Options.Source, Options.Build, Options.RootDir, Options.Test);
        
        // Parser the arguments
        try
        {
            Parser.Parse();
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
        
        // Handle arguments
        if (Parser.GetBool(Options.Help))
        {
            Log.Trace(Parser.GetHelpText());
        }

        // Get predefined sources
        IEnumerable<string> Scripts = Parser.GetValues(Options.Source);

        try
        {
            // Create build task
            BuildTask Task = new(Parser.GetValue<string>(Options.Build));
            if (Scripts is not null) Task.AddScripts(Scripts);
            if (Parser.GetValue<bool>(Options.RootDir))
            {
                Task.RootDirectory = Parser.GetValue<string>(Options.RootDir);
            }
            Task.Compile();
        }
        catch (ScriptCompilationException Exception)
        {
            Log.Error(Exception.Message);
        }

    }
    
    // TODO: Rewrite this Generate method, should write a BuildQueue class that set up things to write vcxproj and sln files
    /*private static void Generate()
    {
        string CurrentDirectory = null;
        Log.Trace($"Current Working Directory: {CurrentDirectory}");
        
        
        
        
        
        
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
    }*/
}

