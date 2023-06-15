using System;
using System.Collections.Generic;
using System.IO;


namespace AstroMake;

public abstract class Application 
{
    protected Workspace Workspace;
    public String Name { get; protected set; }
    public String TargetDirectory { get; protected set; }
    public OutputType Type { get; protected set; }
    public Language Language { get; protected set; }
    public ApplicationFlags Flags { get; protected set; }

    public List<String> Files { get; protected set; }
    public List<String> IncludeDirectories { get; protected set; }
    public List<String> Defines { get; protected set; }
    public List<String> Links { get; protected set; }


    public Application(Workspace Workspace)
    {
        Files = new();
        IncludeDirectories = new();
        Defines = new();
        Links = new();
        Flags = ApplicationFlags.None;
        this.Workspace = Workspace;
    }

    public String GetAbsoluteDirectory()
    {
        return Path.Combine(Directory.GetCurrentDirectory());
    }
}