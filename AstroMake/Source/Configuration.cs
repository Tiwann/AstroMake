using System.Collections.Generic;

namespace AstroMake;


/// <summary>
/// Describes a solution's configurations
/// </summary>
public class Configuration 
{
    public string Name { get; set; }
    public ConfigurationFlags Flags;

    public Configuration(string Name, ConfigurationFlags Flags)
    {
        this.Name = Name;
        this.Flags = Flags;
    }

    public Configuration(string Name)
    {
        this.Name = Name;
        Flags = ConfigurationFlags.None;
    }

    public static List<Configuration> CreateConfigurations(params string[] Configurations)
    {
        List<Configuration> Result = new List<Configuration>();
        foreach (string S in Configurations)
        {
            Result.Add(new Configuration(S));
        }
        return Result;
    }
}