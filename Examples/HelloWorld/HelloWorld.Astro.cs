using AstroMake;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[Build]
public class HelloWorldSolution : Solution
{
    private readonly Configuration Debug = new Configuration("Debug", RuntimeType.Debug, ConfigurationFlags.None);
    private readonly Configuration Release = new Configuration("Release", RuntimeType.Release, ConfigurationFlags.None);
    
    public HelloWorldSolution()
    {
        Name = "HelloWorld";
        AddConfigurations(Debug, Release);
        Architecture = Architecture.x64;
        ProjectNames.Add("HelloWorld");
        TargetDirectory = Location;
    }
}

[Build]
public class HelloWorldProject : ConsoleApplication
{
    public HelloWorldProject(Solution Solution) : base(Solution)
    {
        Name = "HelloWorld";
        Language = Language.CPlusPlus;
        CppStandard = CPPStandard.CPP20;
        Flags = ProjectFlags.MultiProcessorCompile | ProjectFlags.ModuleSupport;
        Location = Path.Combine(Solution.Location, Name);
        TargetDirectory = Location;
        TargetName = "HelloWorld";
        Files.Add(@"Source\HelloWorld.cpp");
        Defines.AddRange(new string[] { "_CRT_SECURE_NO_WARNINGS", "OPENGL", "VULKAN" });
    }

    public override void Configure(Configuration Configuration)
    {
        if (Configuration.Name == "Debug")
        {
            BinariesDirectory = Path.Combine(Solution.Location, "Binaries", "Debug");
            IntermediateDirectory = Path.Combine(Solution.Location, "Intermediate", "Debug");
            Defines.Add("HELLOWORLD_DEBUG");
            return;
        }

        if (Configuration.Name == "Release")
        {
            BinariesDirectory = Path.Combine(Solution.Location, "Binaries", "Release");
            IntermediateDirectory = Path.Combine(Solution.Location, "Intermediate", "Release");
            Defines.Add("HELLOWORLD_RELEASE");
            return;
        }
    }
}