using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace AstroMake;

internal static class Program
{
    private static void Main(string[] Arguments)
    {
        // Hello Astro Make
        Log.Trace($"Astro Make {Version.AstroVersion}");
        Log.Trace("Copyright (C) 2023 Erwann Messoah");
        
        // Setting up the parser
        ArgumentParser Parser = new(Arguments, ArgumentParserSettings.WindowsStyle);
        Parser.AddOptions(Options.Help, Options.Source, Options.Build, Options.RootDir, Options.Clean, Options.Verbose);
        
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

        // Handle Verbosity
        Log.Verbose = Parser.GetValue<bool>(Options.Verbose);
        

        // Handle clean
        if (Parser.GetValue<bool>(Options.Clean))
        {
            Log.Trace("> Cleaning all generated files...");
            if (!File.Exists("astromake"))
            {
                Log.Error("config.astro file not found!");
                Environment.Exit(-1);
            }

            List<string> FileContent = File.ReadLines("astromake").ToList();
            IEnumerable<string> Files = FileContent.Where(S => !S.StartsWith("#") && File.Exists(S));
            IEnumerable<string> Directories = FileContent.Where(S => !S.StartsWith("#") && Directory.Exists(S));
            foreach (string F in Files)
            {
                Log.Trace($"> Deleting {F}");
                File.Delete(F);
            }
        
            foreach (string D in Directories)
            {
                Log.Trace($"> Deleting {D}");
                Directory.Delete(D);
            }
        
            File.Delete("astromake");
            Log.Success("> Done!");
            Environment.Exit(0);
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
                Environment.Exit(0);
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
            catch (NotImplementedException)
            {
                Log.Error("This function is not yet available. Please wait for future updates.");
                Environment.Exit(0);
            }
        }


    }
}

