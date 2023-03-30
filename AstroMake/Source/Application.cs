using System;
using System.Collections.Generic;

namespace AstroMake
{
    /// <summary>
    /// Describe an Application (Project) properties
    /// </summary>
    public class Application 
    {
        
        public String Workspace { get; set; }
        public String Name { get; protected set; }
        public String Directory { get; protected set; }
        public String TargetDirectory { get; protected set; }
        public String IntermediateDirectory { get; protected set; }
        public String DefaultDirectory => $"{Workspace}/{Name}";
        public String DefaultTargetDirectory => $"{DefaultDirectory}/Binaries/";
        public String DefaultIntermediateDirectory => $"{DefaultDirectory}/Intermediate/";
        
        public OutputType Type { get; protected set; }
        public Language Language { get; protected set; }
        public ApplicationFlags Flags { get; protected set; }

        public List<String> Files { get; protected set; }
        public List<String> IncludeDirectories { get; protected set; }
        public List<String> Defines { get; protected set; }
        public List<String> Links { get; protected set; }


        public Application()
        {
            Directory = DefaultDirectory;
            TargetDirectory = DefaultTargetDirectory;
            IntermediateDirectory = DefaultIntermediateDirectory;
        }
        
        public bool IsValid() => Workspace != null && !Helpers.AllStringsEmpty(Workspace, Name, Directory);
    }
}
