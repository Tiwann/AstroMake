using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace AstroMake;

internal static class Program
{
    private static void Clean()
    {
        Log.Trace("> Cleaning all generated files...");
        if (!File.Exists("astromake"))
        {
            Log.Error("config.astro file not found!");
            Environment.Exit(-1);
        }
        
        IEnumerable<string> Files = File.ReadLines("astromake").Where(S => !S.StartsWith("#") && File.Exists(S));

        foreach (string F in Files)
        {
            Log.Trace($"> Deleting {F}");
            File.Delete(F);
        }
        File.Delete("config.astro");
        Log.Success("> Done!");
        Environment.Exit(0);
    }
    
    private static void Main(string[] Arguments)
    {
        // Hello Astro Make
        Log.Trace($"Astro Make {Version.AstroVersion}");
        Log.Trace("Copyright (C) 2023 Erwann Messoah");
        Log.Trace("Using dotnet framework 4.8.1\n");

        // Setting up the parser
        ArgumentParser Parser = new(Arguments, ArgumentParserSettings.WindowsStyle);
        Parser.AddOptions(Options.Help, Options.Source, Options.Build, Options.RootDir, Options.Clean);
        
        // Parse the arguments
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
        
        // Handle help
        if (Parser.GetValue<bool>(Options.Help))
        {
            Log.Trace(Parser.GetHelpText());
        }

        // Handle clean
        if (Parser.GetValue<bool>(Options.Clean))
        {
            Clean();
        }

        if (Parser.GetBool(Options.Build))
        {
            // Handle build
            try
            {
                // Create build task
                IEnumerable<string> PredefinedSources = Parser.GetValues(Options.Source);
                BuildTask Task = new BuildTask(Parser.GetValue<string>(Options.Build), PredefinedSources);
                if (Parser.GetValue<bool>(Options.RootDir))
                {
                    Task.RootDirectory = Parser.GetValue<string>(Options.RootDir);
                }

                Task.Compile();
                Task.Build();
            }
            catch (ScriptCompilationFailedException Exception)
            {
                Log.Error(Exception.Message);
            }
            catch (BuildFailedException Exception)
            {
                Log.Error(Exception.Message);
                Environment.Exit(-1);
            }
            catch (DirectoryNotFoundException Exception)
            {
                Log.Error(Exception.Message);
            }
        }


    }
}

