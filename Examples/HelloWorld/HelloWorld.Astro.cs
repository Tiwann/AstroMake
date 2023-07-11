using AstroMake;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[Build]
public class HelloWorldSolution : Solution
{
    public HelloWorldSolution()
    {
        Name = "HelloWorld";
        Configurations = Configuration.CreateConfigurations("Debug", "Release");
        Architecture = Architecture.x64;
        ProjectNames.Add("HelloWorld");
        TargetDirectory = Location;
    }
}

[Build]
public class HelloWorldProject : StaticLibrary
{
    public HelloWorldProject(Solution Solution) : base(Solution)
    {
        Name = "HelloWorld";
        Language = Language.CPlusPlus;
        CppStandard = CPPStandard.CPP20;
        Flags = ProjectFlags.MultiProcessorCompile;
        Location = Path.Combine(Solution.Location, Name);
        TargetDirectory = Location;
        TargetName = "HelloWorld";
        Files.Add(@"Source\HelloWorld.cpp");
		BinariesDirectory = Path.Combine(Solution.Location, "Binaries");
		IntermediateDirectory = Path.Combine(Solution.Location, "Intermediate");
    }
}