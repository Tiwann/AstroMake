using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace AstroMake;

public abstract class Workspace 
{
    public String Name;
    public String TargetDirectory;
    public List<Configuration> Configurations;
    public List<String> Platforms;
    public List<System> Systems;
    public List<Architecture> Architectures;
    public List<String> Applications;

    public Workspace()
    {
        Configurations = new();
        Systems = new();
        Architectures = new();
        Applications = new();
        Platforms = new();
        TargetDirectory = Directory.GetCurrentDirectory();
    }
    
    public IEnumerable<String> GetConfigurationsNames()
    {
        IEnumerable<String> Result = new List<String>();

        if (Platforms.Count == 0)
        {
            foreach (var Config in Configurations)
            {
                foreach (var Architecture in Architectures)
                {
                    Result.ToList().Add($"{Config.Name}|{Architecture}");
                }
            }
        }
        else
        {
            foreach (var Config in Configurations)
            {
                foreach (var Platform in Platforms)
                {
                    foreach (var Architecture in Architectures)
                    {
                        Result.ToList().Add($"{Platform} {Config.Name}|{Architecture}");
                    }
                }
            }
        }
        

        return Result;
    }
}