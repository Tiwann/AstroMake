using System;

namespace AstroMake;


/// <summary>
/// Describes a solution's configurations
/// </summary>
public class Configuration 
{
    /// <summary>
    /// Configuration's name
    /// </summary>
    public string Name;
    
    /// <summary>
    /// Flags
    /// <see cref="ConfigurationFlags"/>
    /// </summary>
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
}