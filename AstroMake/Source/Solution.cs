using System;
using System.Collections.Generic;
using System.IO;

namespace AstroMake;

/// <summary>
/// Describes a solution/workspace
/// </summary>
public abstract class Solution
{
    public string Name { get; set; }
    public string TargetDirectory { get; protected set; }
    public string Location { get; protected set; } = Directory.GetCurrentDirectory();
    public List<Configuration> Configurations { get; protected set; }
    public List<string> Platforms { get; set; }
    public List<System> Systems { get; protected set; }
    public Architecture Architecture { get; protected set; } = Architecture.x64;
    public List<string> ProjectNames { get; protected set; }
    public List<Project> Projects { get; protected set; }

    public string AstroMakeFilePath => Path.Combine(Location, "astromake");
    
    protected Solution()
    {
        TargetDirectory = Directory.GetCurrentDirectory();
        Configurations = new List<Configuration>();
        Platforms = new List<string>();
        Systems = new List<System>();
        ProjectNames = new List<string>();
        Projects = new List<Project>();
    }
}