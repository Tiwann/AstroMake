using System;
using System.Collections.Generic;

namespace AstroMake 
{
    public class Workspace 
    {
        public String Name;
        public String Directory;
        public List<Configuration> Configurations;
        public Systems Systems;
        public Architectures Architectures;
        public List<String> Applications;

        public Workspace()
        {
            Directory = System.IO.Directory.GetCurrentDirectory();
        }

        public bool IsValid() 
        {
            return !Helpers.AllStringsEmpty(Name, Directory) && Configurations.Count > 0 && Applications.Count > 0;
        }
    }
}
