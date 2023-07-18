using System;
using System.Collections.Generic;
using System.IO;

namespace AstroMake;

/// <summary>
/// Describes an Application/Project
/// </summary>
public abstract class Project
{
    public Solution Solution { get; set; }
    public string Name { get; set; }
    public string TargetDirectory { get; protected set; }
    public string TargetName { get; protected set; } 
    public string Location { get; protected set; }
    public string BinariesDirectory { get; set; }
    public string IntermediateDirectory { get; set; }

    private string Extension
    {
        get
        {
            switch (Language)
            {
                case Language.C:
                case Language.CPlusPlus:
                    return Extensions.VisualCXXProject;
                case Language.CSharp:
                    return Extensions.VisualCSharpProject;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    public string TargetPath => Path.ChangeExtension(Path.Combine(TargetDirectory, TargetName), Extension);
    public OutputType Type { get; protected set; }
    public Language Language { get; protected set; }
    public ProjectFlags Flags { get; protected set; }
    public CPPStandard CppStandard { get; set; } = CPPStandard.CPP20;
    public CStandard CStandard { get; set; } = CStandard.C17;
    public CSharpVersion CSharpVersion { get; set; } = CSharpVersion.CSharp11;
    public List<string> Files { get; protected set; }
    public List<string> AdditionalFiles { get; protected set; }
    public List<string> IncludeDirectories { get; protected set; }
    public List<string> LibrariesDirectories { get; protected set; }
    public List<string> Defines { get; protected set; }
    public List<string> Links { get; protected set; }
    public Guid Guid { get; }
    
    public Guid ProjectTypeGuid
    {
        get
        {
            switch (Language)
            {
                case Language.C:
                case Language.CPlusPlus:
                    return new Guid("8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942");
                case Language.CSharp:
                    return new Guid("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    protected Project(Solution Solution)
    {
        Files = new();
        AdditionalFiles = new();
        IncludeDirectories = new();
        LibrariesDirectories = new();
        Defines = new();
        Links = new();
        Flags = ProjectFlags.None;
        this.Solution = Solution;
        Guid = Guid.NewGuid();
    }
    
    public virtual void Configure(Configuration Configuration)
    {
        
    }
}