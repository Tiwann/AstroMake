using System;
using System.Collections.Generic;

namespace AstroMake 
{
    public class Workspace 
    {
        public String Name;
        public String Directory;
        public List<Configuration> Configurations;
        public System Systems;
        public Architecture Architectures;
        public List<Application> Applications;

        public Workspace(String Name) => this.Name = Name;

        public bool IsValid() 
        {
            return !Helpers.AllStringsEmpty(Name, Directory) && Configurations.Count > 0 && Applications.Count > 0;
        }
    }
}
