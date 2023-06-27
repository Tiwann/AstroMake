using System;
using System.Collections.Generic;
using System.IO;

namespace AstroMake;


/// <summary>
/// Describes an Application/Project
/// </summary>
public abstract class Project 
{
    /// <summary>
    /// Reference to a <see cref="Solution"/>
    /// </summary>
    public Solution Solution;
    
    /// <summary>
    /// Project name
    /// </summary>
    public string Name { get; protected set; }
    
    /// <summary>
    /// Project target directory
    /// </summary>
    public string TargetDirectory { get; protected set; }
    
    
    /// <summary>
    /// Project <see cref="OutputType"/>
    /// </summary>
    public OutputType Type { get; set; }
    
    
    /// <summary>
    /// Project <see cref="Language"/>
    /// </summary>
    public Language Language { get; protected set; }
    
    
    /// <summary>
    /// <see cref="ProjectFlags"/>
    /// </summary>
    protected ProjectFlags Flags { get; set; }

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
    /// List of Projects to link against. Should be application's name
    /// </summary>
    public List<string> Links { get; protected set; }
    
    public Guid GUID => Guid.NewGuid();
    
    protected Project(Solution Solution)
    {
        Files = new();
        IncludeDirectories = new();
        Defines = new();
        Links = new();
        Flags = ProjectFlags.None;
        this.Solution = Solution;
        
    }
}