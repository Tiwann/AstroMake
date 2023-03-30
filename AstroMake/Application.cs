using System;
using System.Collections.Generic;
using System.Linq;

namespace AstroMake
{
    public class Application 
    {
        public Workspace Workspace;
        public String Name;
        public String Directory;
        public String TargetDirectory;
        public String IntermediateDirectory;

        String DefaultDirectory => $"{Workspace.Directory}/{Name}";
        String DefaultTargetDirectory => $"{DefaultDirectory}/Binaries/";
        String DefaultIntermediateDirectory => $"{DefaultDirectory}/Intermediate/";
        
        public ApplicationType Type;
        public Language Language;

        public List<String> Files;
        public List<String> IncludeDirectories;
        public List<String> Defines;
        public List<Application> Links;
        

        public bool IsValid() 
        {
            return Workspace != null && Workspace.IsValid() && !Helpers.AllStringsEmpty(Name, Directory);
        }
    }
}
