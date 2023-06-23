using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace AstroMake;


/// <summary>
/// Describes a solution/workspace
/// </summary>
public abstract class Solution 
{
    /// <summary>
    /// Solution name
    /// </summary>
    public string Name;
    
    /// <summary>
    /// Solution's target directory
    /// </summary>
    public string TargetDirectory;
    
    /// <summary>
    /// Configurations
    /// </summary>
    public List<Configuration> Configurations { get; protected set; }
    
    /// <summary>
    /// Platforms.
    /// </summary>
    public List<string> Platforms { get; protected set; }
    
    /// <summary>
    /// Systems.
    /// </summary>
    public List<System> Systems { get; protected set; }
    
    /// <summary>
    /// Architectures
    /// </summary>
    public List<Architecture> Architectures;
    
    /// <summary>
    /// Applications linked to this solutions
    /// </summary>
    public List<string> Applications { get; set; }

    protected Solution()
    {
        Configurations = new();
        Systems = new();
        Architectures = new();
        Applications = new();
        Platforms = new();
        TargetDirectory = Directory.GetCurrentDirectory();
    }
}