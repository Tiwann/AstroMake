using System;
using System.Collections.Generic;

namespace AstroMake;

public class Configuration 
{
    public String Name;
    public ConfigurationFlags Flags;

    public Configuration(String Name, ConfigurationFlags Flags)
    {
        this.Name = Name;
        this.Flags = Flags;
    }

    public Configuration(String Name)
    {
        this.Name = Name;
        Flags = ConfigurationFlags.None;
    }
}