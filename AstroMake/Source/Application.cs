using System;
using System.Collections.Generic;

namespace AstroMake;


/// <summary>
/// Describes an Application/Project
/// </summary>
public abstract class Application 
{
    /// <summary>
    /// Reference to a <see cref="Solution"/>
    /// </summary>
    protected Solution Solution;
    
    /// <summary>
    /// Application name
    /// </summary>
    protected string Name { get; set; }
    
    /// <summary>
    /// Application target directory
    /// </summary>
    public string TargetDirectory { get; protected set; }
    
    
    /// <summary>
    /// Application <see cref="OutputType"/>
    /// </summary>
    protected OutputType Type { get; set; }
    
    
    /// <summary>
    /// Application <see cref="Language"/>
    /// </summary>
    public Language Language { get; protected set; }
    
    
    /// <summary>
    /// <see cref="ApplicationFlags"/>
    /// </summary>
    protected ApplicationFlags Flags { get; set; }

    /// <summary>
    /// List of files to include. Entries could be absolute filepaths, relative filepaths, or wildcards
    /// </summary>
    protected List<string> Files { get; set; }
    
    /// <summary>
    /// List of Include directories. Entries could be absolute paths, relative paths, or wildcards
    /// </summary>
    protected List<string> IncludeDirectories { get; set; }
    
    /// <summary>
    /// List of preprocessor defines. 
    /// </summary>
    protected List<string> Defines { get; set; }
    
    /// <summary>
    /// List of Applications to link against. Should be application's name
    /// </summary>
    public List<string> Links { get; protected set; }


    protected Application(Solution Solution)
    {
        Files = new();
        IncludeDirectories = new();
        Defines = new();
        Links = new();
        Flags = ApplicationFlags.None;
        this.Solution = Solution;
    }
}