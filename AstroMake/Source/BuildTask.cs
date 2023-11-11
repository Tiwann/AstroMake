using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
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
    private EmitResult CompilerResults;
    private Assembly CompiledAssembly;

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
        FoundBuildScripts.ForEach(B => Log.Trace($"--> {B}"));
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

        string[] ReferenceList =
        {
            "System.dll",
            "System.Private.CoreLib.dll",
            "System.Runtime.dll",
            "System.Console.dll",
            "netstandard.dll",
            "System.Text.RegularExpressions.dll",
            "System.Linq.dll",
            "System.Linq.Expressions.dll",
            "System.IO.dll",
            "System.Reflection.dll",
            "System.Collections.dll",
            "System.Collections.Concurrent.dll",
            "System.Collections.NonGeneric.dll",
            "System.Collections.Specialized.dll",
            "Microsoft.CSharp.dll"
        };

        List<MetadataReference> GetReferences(IEnumerable<string> Dlls)
        {
            List<MetadataReference> Out = new List<MetadataReference>();
            string ReferencesPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            foreach (string Dll in Dlls)
            {
                if (ReferencesPath != null)
                    Out.Add(MetadataReference.CreateFromFile(Path.Combine(ReferencesPath, Dll)));
            }
            return Out;
        }
        
        List<string> Sources = BuildScripts.Select(File.ReadAllText).ToList();
        List<MetadataReference> References = GetReferences(ReferenceList);
        References.Add(MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location));
        List<SyntaxTree> SyntaxTrees = new List<SyntaxTree>();
        Sources.ForEach(Source => SyntaxTrees.Add(SyntaxFactory.ParseSyntaxTree(Source)));
        CSharpCompilation Compiler = CSharpCompilation.Create("AstroMakeRuntimeCompiler")
            .AddSyntaxTrees(SyntaxTrees)
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithOptimizationLevel(OptimizationLevel.Release))
            .AddReferences(References);

        using Stream Output = new MemoryStream();
        CompilerResults = Compiler.Emit(Output);

        if (!CompilerResults.Success)
        {
            StringBuilder Builder = new();
            foreach (Diagnostic CompileError in CompilerResults.Diagnostics)
            {
                Builder.AppendLine($"{CompileError}.");
            }
            throw new ScriptCompilationFailedException(Builder.ToString());
        }

        CompiledAssembly = Assembly.Load(((MemoryStream)Output).ToArray());
        Stopwatch.Stop();
        Log.Success($"> Compilation successful! Took {Stopwatch.ElapsedMilliseconds / 1000.0f}s.");
    }

    public void Build()
    {
        if (CompiledAssembly is null) throw new BuildFailedException("Compiled Assembly is null!");
        Type SolutionType = CompiledAssembly.GetTypes().Single(Type =>
            Type.IsSubclassOf<Solution>() &&
            Type.GetCustomAttribute<BuildAttribute>() is not null);
        
        Solution Solution = SolutionType.CreateInstance<Solution>()!;
        if (Solution is null)
        {
            throw new BuildFailedException("Failed to instantiate Solution!");
        }

        if (string.IsNullOrEmpty(Solution.Name))
        {
            throw new BuildFailedException($"Solution \"{Solution.GetType().Name}\" needs a name.");
        }
        
        if (Solution.Name.Contains(' '))
        {
            string NewName = Solution.Name.Replace(' ', '_');
            Log.Warning($"> Solution \"{Solution.Name}\" was renamed to \"{NewName}\".");
            Solution.Name = NewName;
        }
        
        Solution.Configurations.ForEach(Conf =>
        {
            if (Conf.Name.Contains(' '))
            {
                string NewName = Conf.Name.Replace(' ', '_');
                Log.Warning($"Project \"{Solution.Name}\" was renamed to \"{NewName}\".");
                Conf.Name = NewName;
            }
        });
        
        if (string.IsNullOrEmpty(Solution.TargetDirectory))
        {
            throw new BuildFailedException($"Please specify a target directory for solution \"{Solution.Name}\"");
        }
        
        if (Solution.Configurations.IsEmpty())
        {
            throw new BuildFailedException($"Solution \"{Solution.GetType().Name}\" needs a set of configurations.");
        }

        if (!Solution.Systems.IsEmpty())
        {
            var NewPlatforms = new List<string>();
            Solution.Systems.ForEach(Sys =>
            {
                if (!Solution.Platforms.IsEmpty())
                {
                    Solution.Platforms.ForEach(Platform =>
                    {
                        NewPlatforms.Add($"{Platform}{Sys.ToString()}");
                    });
                }
                else
                {
                    NewPlatforms.Add(Sys.ToString());
                }
            });
            Solution.Platforms = NewPlatforms;
        }
        
        
        List<Type> ProjectTypes = CompiledAssembly.GetTypes().Where(Type =>
            Type.IsSubclassOf<Project>() &&
            Type.GetCustomAttribute<BuildAttribute>() != null).ToList();

        if (ProjectTypes.IsEmpty()) throw new BuildFailedException("No Projects found!");
        
        foreach (Type Type in ProjectTypes)
        {
            Project ProjectInstance = Type.CreateInstance<Project>(Solution)!;
            if (Solution.ProjectNames.Contains(ProjectInstance.Name))
            {
                Solution.Projects.Add(ProjectInstance);
            }
        }
        
        Solution.Projects.ForEach(Project =>
        {
            if (string.IsNullOrEmpty(Project.Name))
            {
                throw new BuildFailedException($"Project \"{Project.GetType().Name}\" needs a name.");
            }

            if (Project.Name.Contains(' '))
            {
                string NewName = Project.Name.Replace(' ', '_');
                Log.Warning($"Project \"{Project.Name}\" was renamed to \"{NewName}\".");
                Project.Name = NewName;
            }
            
            if (string.IsNullOrEmpty(Project.TargetDirectory))
            {
                throw new BuildFailedException($"Please specify a target directory for project \"{Project.Name}\".");
            }
            
            if (string.IsNullOrEmpty(Project.Location))
            {
                throw new BuildFailedException($"Please specify a location for project \"{Project.Name}\".");
            }
        });

        switch (BuildType)
        {
            case BuildTaskType.VisualStudioSolution:
                BuildVisualStudioSolution(Solution);
                break;
            case BuildTaskType.Makefiles:
                BuildMakefiles(Solution);
                break;
            case BuildTaskType.XCodeProject:
                BuildXCodeProject(Solution);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    private void BuildVisualStudioSolution(Solution Solution)
    {
        Log.Trace("> Generating Visual Studio Solution...");
        Stopwatch Stopwatch = new();
        Stopwatch.Start();

        // Write vcxproj files
        List<string> GeneratedFiles = new List<string>();
        foreach (Project Project in Solution.Projects)
        {
            if (!Directory.Exists(Project.TargetDirectory))
            {
                Directory.CreateDirectory(Project.TargetDirectory);
            }
            
            string Filepath = Path.ChangeExtension(Project.TargetPath, Extensions.VisualCXXProject);
            GeneratedFiles.Add(Filepath);
            using FileStream Stream = File.Open(Filepath, FileMode.OpenOrCreate, FileAccess.Write);
            Stream.SetLength(0);
            using VcxprojWriter Writer = new(Stream, Project);
            Writer.Write();
            Log.Trace($"> Generated {Filepath}");
        }

        // Write Sln file
        if (!Directory.Exists(Solution.TargetDirectory))
            Directory.CreateDirectory(Solution.TargetDirectory);
        
        string SlnFilepath = $"{Solution.TargetDirectory}\\{Solution.Name}{Extensions.VisualStudioSolution}";
        GeneratedFiles.Add(SlnFilepath);
        using FileStream SlnStream = new (SlnFilepath, FileMode.OpenOrCreate, FileAccess.Write);
        using SlnWriter SlnWriter = new (this, SlnStream, Solution);
        SlnStream.SetLength(0);
        SlnWriter.Write();
        Log.Trace($"> Generated {SlnFilepath}");

        Log.Trace($"> Writing {Solution.AstroMakeFilePath}...");
        CreateAstroMakeFile(GeneratedFiles, Solution);
        Stopwatch.Stop();
        Log.Success($"> Visual Studio Solution generation successful! Took {Stopwatch.ElapsedMilliseconds / 1000.0f}s");
    }

    private void BuildMakefiles(Solution Solution)
    {
        throw new NotImplementedException();
    }

    private void BuildXCodeProject(Solution Solution)
    {
        throw new NotImplementedException();
    }

    private void CreateAstroMakeFile(IEnumerable<string> GeneratedFiles, Solution Solution)
    {
        string Filepath = Solution.AstroMakeFilePath;
        StringBuilder Builder = new ();
        Builder.AppendLine($"# Astro Make {Version.AstroVersion}");
        Builder.AppendLine("# (C) Erwann Messoah 2023");
        Builder.AppendLine("# https://github.com/Tiwann/AstroMake\n");
        foreach (string GeneratedFile in GeneratedFiles)
        {
            Builder.AppendLine(GeneratedFile);
        }

        if (Solution.TargetDirectory != RootDirectory)
        {
            Builder.AppendLine(Solution.TargetDirectory);
        }
        File.WriteAllText(Filepath, Builder.ToString());
    }
}