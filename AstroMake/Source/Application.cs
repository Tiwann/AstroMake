using System;
using System.Collections.Generic;

namespace AstroMake
{
    public class Application 
    {
        public Workspace Workspace { get; set; }
        public String Name;
        public String Directory;
        public String TargetDirectory;
        public String IntermediateDirectory;
        public String DefaultDirectory => $"{Workspace.Directory}/{Name}";
        public String DefaultTargetDirectory => $"{DefaultDirectory}/Binaries/";
        public String DefaultIntermediateDirectory => $"{DefaultDirectory}/Intermediate/";

        
        
        public ApplicationType Type;
        public Language Language;

        public List<String> Files;
        public List<String> IncludeDirectories;
        public List<String> Defines;
        public List<String> Links;


        public Application()
        {
            Directory = DefaultDirectory;
            TargetDirectory = DefaultTargetDirectory;
            IntermediateDirectory = DefaultIntermediateDirectory;
        }
        

        public bool IsValid() => Workspace != null && Workspace.IsValid() && !Helpers.AllStringsEmpty(Name, Directory);
    }
}
