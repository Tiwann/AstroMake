using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace AstroMake;

public enum BuildTaskType
{
    VisualStudioSolution,
    Makefiles,
    XCodeProject
}

public class BuildTask
{
    private readonly BuildTaskType BuildType;
    private readonly List<string> BuildScripts = new();
    public string RootDirectory { get; set; } = Directory.GetCurrentDirectory();
    private CompilerResults CompilerResults;

    public BuildTask(string BuildType, IEnumerable<string> PredefinedSource = null)
    {
        switch (BuildType)
        {
            case "vstudio":
                this.BuildType = BuildTaskType.VisualStudioSolution;
                break;
            case "makefile":
                this.BuildType = BuildTaskType.Makefiles;
                break;
            case "xcode":
                this.BuildType = BuildTaskType.XCodeProject;
                break;
        }
        
        if(PredefinedSource is null) SearchScripts(); else AddScripts(PredefinedSource);
    }

    private void SearchScripts()
    {
        Log.Trace("> Searching for build scripts...");
        List<string> FoundBuildScripts = Directory.EnumerateFiles(RootDirectory, "*.Astro.cs", SearchOption.AllDirectories).ToList();
        if (FoundBuildScripts.Count <= 0)
        {
            throw new ScriptCompilationFailedException("No build scripts found!");
        }
        
        BuildScripts.AddRange(FoundBuildScripts);
        Log.Trace($"Found {FoundBuildScripts.Count} build script(s):");
        foreach (var BuildFilepath in FoundBuildScripts)
        {
            Log.Trace($"--> {BuildFilepath}");
        }
    }

    public void AddScript(string Script)
    {
        BuildScripts.Add(Script);
    }

    public void AddScripts(IEnumerable<string> Scripts)
    {
        BuildScripts.AddRange(Scripts);
    }

    public void AddScripts(params string[] Scripts)
    {
        BuildScripts.AddRange(Scripts);
    }

    public void Compile()
    {
        if (BuildScripts.Count <= 0)
        {
            throw new ScriptCompilationFailedException("No script found to compile.");
        }
        
        foreach (string BuildScript in BuildScripts)
        {
            if (!File.Exists(BuildScript))
            {
                throw new FileNotFoundException($"File {BuildScript} was not found/doesn't exists!");
            }
        }
        
        Log.Trace("> Compiling scripts...");
        Stopwatch Stopwatch = new();
        Stopwatch.Start();
        using CSharpCodeProvider CodeProvider = new();
        CompilerParameters Parameters = new()
        {
            GenerateInMemory = true,
            GenerateExecutable = false,
            IncludeDebugInformation = true,
            ReferencedAssemblies = { Assembly.GetExecutingAssembly().Location }
        };
        
        CompilerResults = CodeProvider.CompileAssemblyFromFile(Parameters, BuildScripts.ToArray());
        
        if (CompilerResults.Errors.HasErrors)
        {
            StringBuilder Builder = new();
            foreach (CompilerError CompileError in CompilerResults.Errors)
            {
                Builder.AppendLine($"{CompileError.ErrorText}.");
                Builder.AppendLine($"File:{CompileError.FileName}. Line: {CompileError.Line} Col: {CompileError.Column}");
            }
            throw new ScriptCompilationFailedException(Builder.ToString());
        }
        
        Stopwatch.Stop();
        Log.Success($"> Compilation successful! Took {Stopwatch.ElapsedMilliseconds / 1000.0f}s.");
    }

    public void Build()
    {
        switch (BuildType)
        {
            case BuildTaskType.VisualStudioSolution:
                Log.Trace("> Generating Visual Studio Solution...");
                break;
            case BuildTaskType.Makefiles:
                Log.Trace("> Generating Makefiles...");
                break;
            case BuildTaskType.XCodeProject:
                Log.Trace("> Generating XCode Project...");
                break;
        }
        
        Assembly CompiledAssembly = CompilerResults.CompiledAssembly;

        Type SolutionType = CompiledAssembly.GetTypes().Single(Type => 
            Type.IsSubclassOf<Solution>() &&
            Type.GetCustomAttribute<BuildAttribute>() != null);
        
        Solution Solution = Activator.CreateInstance(SolutionType) as Solution;

        
        IEnumerable<Type> ApplicationsTypes = CompiledAssembly.GetTypes().Where(Type =>
            Type.IsSubclassOf<Application>() &&
            Type.GetCustomAttribute<BuildAttribute>() != null);
        
        List<Application> Applications = new List<Application>();
        
        foreach (Type Type in ApplicationsTypes)
        {
            Application ApplicationInstance = Type.CreateInstance<Application>(Solution);
            if (Solution.Applications.Contains(ApplicationInstance.Name))
            {
                Applications.Add(ApplicationInstance);
            }
        }

        switch (BuildType)
        {
            case BuildTaskType.VisualStudioSolution:
                BuildVisualStudioSolution(Solution, Applications);
                break;
            case BuildTaskType.Makefiles:
                BuildMakefiles(Solution, Applications);
                break;
            case BuildTaskType.XCodeProject:
                BuildXCodeProject(Solution, Applications);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    private void BuildVisualStudioSolution(Solution Solution, IEnumerable<Application> Applications)
    {
        Stopwatch Stopwatch = new Stopwatch();
        Stopwatch.Start();

        List<string> GeneratedFiles = new List<string>();
        // Write vcxproj files
        foreach (Application Application in Applications)
        {
            if (!Directory.Exists(Application.TargetDirectory))
            {
                throw new DirectoryNotFoundException($"The target directory of application \"{Application.Name}\" was not found.");
            }

            string Filepath = Path.ChangeExtension(Path.Combine(Solution.TargetDirectory, Application.Name, Application.Name), Extensions.VisualCXXProject);
            GeneratedFiles.Add(Filepath);
            using FileStream Stream = File.Open(Filepath, FileMode.OpenOrCreate, FileAccess.Write);
            using VcxprojWriter Writer = new VcxprojWriter(Stream, Application);
            Writer.Write();
            Log.Trace($"> Generated {Filepath}");
        }
        
        Log.Trace("> Writing .AstroMake file...");
        CreateAstroMakeFile(GeneratedFiles);
        Stopwatch.Stop();
        Log.Success($"> Visual Studio Solution generation successful! Took {Stopwatch.ElapsedMilliseconds / 1000.0f}s");
        
    }

    private void BuildMakefiles(Solution Solution, List<Application> Applications)
    {
        throw new NotImplementedException();
    }

    private void BuildXCodeProject(Solution Solution, List<Application> Applications)
    {
        throw new NotImplementedException();
    }

    private void CreateAstroMakeFile(IEnumerable<string> GeneratedFiles)
    {
        string Filepath = $"{Directory.GetCurrentDirectory()}\\.AstroMake";
        StringBuilder Builder = new StringBuilder();
        Builder.AppendLine($"# Astro Make {Version.AstroVersion}");
        Builder.AppendLine("# (C) Erwann Messoah 2023");
        Builder.AppendLine("# https://github.com/Tiwann/AstroMake\n");
        foreach (string GeneratedFile in GeneratedFiles)
        {
            Builder.AppendLine(GeneratedFile);
        }
        File.WriteAllText(Filepath, Builder.ToString());
    }
}