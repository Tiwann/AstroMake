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
    private readonly BuildTaskType Type;
    private readonly List<string> BuildScripts = new();
    public string RootDirectory { get; set; } = Directory.GetCurrentDirectory();

    public BuildTask(string BuildType, bool SearchForScripts = false)
    {
        switch (BuildType)
        {
            case "vstudio":
                Type = BuildTaskType.VisualStudioSolution;
                break;
            case "makefile":
                Type = BuildTaskType.Makefiles;
                break;
            case "xcode":
                Type = BuildTaskType.XCodeProject;
                break;
        }

        if(SearchForScripts) SearchScripts();
    }

    private void SearchScripts()
    {
        Log.Trace("Searching for build scripts...");
        List<string> FoundBuildScripts = Directory.EnumerateFiles(RootDirectory, "*.Astro.cs", SearchOption.AllDirectories).ToList();
        if (FoundBuildScripts.Count <= 0)
        {
            //TODO: Throw exception instead ?
            Log.Error("No build scripts found!");
            Environment.Exit(-1);
            return;
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

    public void Compile()
    {
        if (BuildScripts.Count <= 0)
        {
            throw new ScriptCompilationException("No script found to compile.");
        }
        
        Log.Trace("Compiling scripts...");
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
        
        CompilerResults CompileResults = CodeProvider.CompileAssemblyFromFile(Parameters, BuildScripts.ToArray());
        
        if (CompileResults.Errors.HasErrors)
        {
            StringBuilder Builder = new();
            foreach (CompilerError CompileError in CompileResults.Errors)
            {
                Builder.AppendLine($"{CompileError.ErrorText}.");
                Builder.AppendLine($"File:{CompileError.FileName}. Line: {CompileError.Line} Col: {CompileError.Column}");
            }
            throw new ScriptCompilationException(Builder.ToString());
        }
        
        Stopwatch.Stop();
        Log.Success($"Compilation successful! Took {Stopwatch.ElapsedMilliseconds / 1000.0f}s.");
    }
}