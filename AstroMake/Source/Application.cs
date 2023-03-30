using System;
using System.Collections.Generic;

namespace AstroMake
{
    /// <summary>
    /// Describe an Application (Project) properties
    /// <param name="Workspace">The workspace it belongs to</param>
    /// <param name="Name">Name of the project</param>
    /// <param name="Directory">Absolute path of the program</param>
    /// <param name="TargetDirectory">Binaries output path</param>
    /// <param name="Intermediate">Intermediate (Objects) output path</param>
    /// <param name="Type">Whether it is a Console Application, Windowed Application, Static Library or Shared Library</param>
    /// <param name="Language">Programming language. Currently supports C, C++, and C#</param>
    /// <param name="Files">List of files to include. Possibilty to use wildcards such as: "Source/*.cpp" to include all .cpp files inside Source folder</param>
    /// <param name="IncludeDirectories">List of include directories</param>
    /// <param name="Defines">List of preprocessing definitions</param>
    /// <param name="Links">List of dependencies to link to</param>
    /// </summary>
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

        
        
        public OutputType Type;
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
