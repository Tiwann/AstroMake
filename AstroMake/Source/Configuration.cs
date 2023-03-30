using System;
using System.Collections.Generic;

namespace AstroMake 
{
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

        public static List<Configuration> CreateConfigurations(params String[] ConfigName)
        {
            List<Configuration> Configs = new List<Configuration>();
            foreach (String Str in ConfigName)
                Configs.Add(new Configuration(Str));
            return Configs;
        }
    }
}
