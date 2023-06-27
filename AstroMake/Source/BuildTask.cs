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
    public List<string> BuildScripts { get; } = new();
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
                Builder.AppendLine($"File: {CompileError.FileName}. Line: {CompileError.Line} Col: {CompileError.Column}");
            }
            throw new ScriptCompilationFailedException(Builder.ToString());
        }
        
        Stopwatch.Stop();
        Log.Success($"> Compilation successful! Took {Stopwatch.ElapsedMilliseconds / 1000.0f}s.");
    }

    public void Build()
    {
        Assembly CompiledAssembly = CompilerResults.CompiledAssembly;

        Type SolutionType = CompiledAssembly.GetTypes().Single(Type => 
            Type.IsSubclassOf<Solution>() &&
            Type.GetCustomAttribute<BuildAttribute>() != null);
        
        Solution Solution = Activator.CreateInstance(SolutionType) as Solution;

        
        IEnumerable<Type> ApplicationsTypes = CompiledAssembly.GetTypes().Where(Type =>
            Type.IsSubclassOf<Project>() &&
            Type.GetCustomAttribute<BuildAttribute>() != null);
        
        List<Project> Projects = new List<Project>();
        
        foreach (Type Type in ApplicationsTypes)
        {
            Project ProjectInstance = Type.CreateInstance<Project>(Solution);
            if (Solution.ApplicationNames.Contains(ProjectInstance.Name))
            {
                Projects.Add(ProjectInstance);
                Solution.Projects.Add(ProjectInstance);
            }
        }

        switch (BuildType)
        {
            case BuildTaskType.VisualStudioSolution:
                BuildVisualStudioSolution(Solution, Projects);
                break;
            case BuildTaskType.Makefiles:
                BuildMakefiles(Solution, Projects);
                break;
            case BuildTaskType.XCodeProject:
                BuildXCodeProject(Solution, Projects);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    private void BuildVisualStudioSolution(Solution Solution, IEnumerable<Project> Projects)
    {
        Log.Trace("> Generating Visual Studio Solution...");
        Stopwatch Stopwatch = new Stopwatch();
        Stopwatch.Start();

        // Write vcxproj files
        List<string> GeneratedFiles = new List<string>();
        foreach (Project Project in Projects)
        {
            if (!Directory.Exists(Project.TargetDirectory))
            {
                throw new DirectoryNotFoundException($"The target directory of application \"{Project.Name}\" was not found.");
            }
            
            string Filepath = Path.ChangeExtension(Path.Combine(Solution.TargetDirectory, Project.Name, Project.Name), Extensions.VisualCXXProject);
            GeneratedFiles.Add(Filepath);
            using FileStream Stream = File.Open(Filepath, FileMode.OpenOrCreate, FileAccess.Write);
            using VcxprojWriter Writer = new VcxprojWriter(Stream, Project);
            Writer.Write();
            Log.Trace($"> Generated {Filepath}");
        }

        // Write Sln file
        string SlnFilepath = $"{Solution.TargetDirectory}\\{Solution.Name}{Extensions.VisualCXXSolution}";
        GeneratedFiles.Add(SlnFilepath);
        using FileStream SlnStream = new (SlnFilepath, FileMode.OpenOrCreate, FileAccess.Write);
        using SlnWriter SlnWriter = new (this, SlnStream, Solution);
        SlnWriter.Write();
        Log.Trace($"> Generated {SlnFilepath}");
        
        Log.Trace("> Writing .AstroMake file...");
        CreateAstroMakeFile(GeneratedFiles);
        Stopwatch.Stop();
        Log.Success($"> Visual Studio Solution generation successful! Took {Stopwatch.ElapsedMilliseconds / 1000.0f}s");
        
    }

    private void BuildMakefiles(Solution Solution, List<Project> Projects)
    {
        throw new NotImplementedException();
    }

    private void BuildXCodeProject(Solution Solution, List<Project> Projects)
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