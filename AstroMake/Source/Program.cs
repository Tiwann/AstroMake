using System.Diagnostics;


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
        Parser.AddOptions(
            Options.Help,
            Options.Init,
            Options.Source, 
            Options.Build, 
            Options.RootDir, 
            Options.Clean, 
            Options.Verbose,
            Options.Install);
        
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
            Environment.Exit(0);
        }

        if (Parser.GetValue<bool>(Options.Install))
        {
            Log.Error("This feature is not implemented yet.");
            Environment.Exit(0);
            Log.Trace("Are you sure you wanna install AstroMake into your system ?");
            Log.Trace("> Yes (y) | No (n)");
            ConsoleKey Key = ConsoleExtensions.WaitForKeys(ConsoleKey.Y, ConsoleKey.N);
            if (Key is ConsoleKey.Y)
            {
                /*HttpClient Client = new HttpClient();
                Uri AstroUrl = new Uri();
                Client.BaseAddress*/
                
            }
        }
        
        // handle init
        if (Parser.GetBool(Options.Init))
        {
            Log.Trace("> Initializing a workspace...");
            Stopwatch Stopwatch = Stopwatch.StartNew();
            var CurrentDir = Directory.GetCurrentDirectory();
            var Files = Directory.GetFiles(CurrentDir);
            var Directories = Directory.GetDirectories(CurrentDir);
            if (!(Files.IsEmpty() && Directories.IsEmpty()))
            {
                Log.Error("Failed to initialize a workspace: Current directory is not empty!");
                Environment.Exit(-1);
            }
            
            string SolutionBoilerplate =
                "using AstroMake;\n" +
                "\n" +
                "[Build]\n" +
                "public class CLASS_NAME_SOLUTION : Solution\n" +
                "{\n" +
                "\tprivate readonly Configuration Debug = new Configuration(\"Debug\", RuntimeType.Debug, ConfigurationFlags.None);\n" +
                "\tprivate readonly Configuration Release = new Configuration(\"Release\", RuntimeType.Release, ConfigurationFlags.None);\n" +
                "\n" +
                "\tpublic CLASS_NAME_SOLUTION()\n" +
                "\t{\n" +
                "\t\tName = \"CLASS_NAME\";\n" +
                "\t\tAddConfigurations(Debug, Release);\n" +
                "\t\tArchitecture = Architecture.x64;\n" +
                "\t\tTargetDirectory = Location;\n\n" +
                "\t\tPreBuildCommands.Add(\"explorer .\");\n" +
                "\t\t// Add projects to the solution here (Names should be same as Project.Name)\n" +
                "\t\tProjectNames.Add(\"MyProject\");\n" +
                "\t}\n" +
                "}\n";

            string Name = Parser.GetValue<string>(Options.Init);
            SolutionBoilerplate = SolutionBoilerplate
                .Replace("CLASS_NAME_SOLUTION", $"{Name}Solution")
                .Replace("CLASS_NAME", $"{Name}");

            {
                string SolutionFilepath = Path.Combine(CurrentDir, $"{Name}{Extensions.AstroBuildScript}");
                using FileStream SolutionFile = new FileStream(SolutionFilepath, FileMode.CreateNew, FileAccess.Write);
                using StreamWriter SolutionWriter = new StreamWriter(SolutionFile);
                SolutionWriter.Write(SolutionBoilerplate);
            }
            
            const string ProjectBoilerplate = "using AstroMake;\n" +
                                              "\n" +
                                              "[Build]\n" +
                                              "public class MyProject : ConsoleApplication\n" +
                                              "{\n" +
                                              "\n" +
                                              "\tpublic MyProject()\n" +
                                              "\t{\n" +
                                              "\t\tName = \"MyProject\";\n" +
                                              "\t\tLanguage = Language.CPlusPlus;\n" +
                                              "\t\tCppStandard = CPPStandard.CPP20;" +
                                              "\n" +
                                              "\t\tFlags = ProjectFlags.MultiProcessorCompile;\n" +
                                              "\t\tTargetDirectory = Location;\n" +
                                              "\t\tTargetName = Name;\n" +
                                              "\t\tFiles.Add(@\"Source\\**.h\");\n" +
                                              "\t\tFiles.Add(@\"Source\\**.cpp\");\n" +
                                              "\t}\n\n" +
                                              "\t// Configure your project based on configuration here\n" +
                                              "\tpublic override void Configure(Configuration Configuration)\n" +
                                              "\t{\n" +
                
                                              "\t}\n" +
                                              "}\n";

            {

                DirectoryInfo MyProjectInfo = Directory.CreateDirectory(Path.Combine(CurrentDir, "MyProject"));
                string ProjectFilepath = Path.Combine(MyProjectInfo.FullName, $"MyProject{Extensions.AstroBuildScript}");
                using FileStream ProjectFile = new FileStream(ProjectFilepath, FileMode.CreateNew, FileAccess.Write);
                using StreamWriter ProjectWriter = new StreamWriter(ProjectFile);
                ProjectWriter.Write(ProjectBoilerplate);
                
                DirectoryInfo SourceInfo = Directory.CreateDirectory(Path.Combine(MyProjectInfo.FullName, "Source"));

                const string MainSource = "#include <iostream>\n" +
                                          "\n" +
                                          "int main(int argc, const char** argv)\n" +
                                          "{\n" +
                                          "\tstd::cout << \"Hello, World!\\n\";\n" +
                                          "}\n";

                string MainSourceFilepath = Path.Combine(SourceInfo.FullName, "Main.cpp");
                using FileStream MainFile = new FileStream(MainSourceFilepath, FileMode.CreateNew, FileAccess.Write);
                using StreamWriter MainWriter = new StreamWriter(MainFile);
                MainFile.SetLength(0);
                MainWriter.Write(MainSource);
                
                Log.Success($"Successfully initialize a workspace! Took {Stopwatch.ElapsedMilliseconds / 1000.0f} seconds");
            }
        }

        // Handle Verbosity
        Log.Verbose = Parser.GetValue<bool>(Options.Verbose);
        
        // Handle clean
        if (Parser.GetValue<bool>(Options.Clean))
        {
            Log.Trace("> Cleaning all generated files...");
            if (!File.Exists(Extensions.AstroFile))
            {
                Log.Error($"{Extensions.AstroFile} file not found!");
                Environment.Exit(-1);
            }

            List<string> FileContent = File.ReadLines(Extensions.AstroFile).ToList();
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
        
            File.Delete(Extensions.AstroFile);
            Log.Success("> Done!");
            Environment.Exit(0);
        }

        // Handle build
        if (Parser.GetBool(Options.Build))
        {
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

