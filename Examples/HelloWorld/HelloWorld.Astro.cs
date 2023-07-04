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
        Configurations = (List<Configuration>)Configuration.CreateConfigurations("Debug", "Release");
        Architecture = Architecture.x64;
        ProjectNames.Add("HelloWorld");
        TargetDirectory = Location;
    }
}

[Build]
public class HelloWorldApplication : ConsoleApplication
{
    public HelloWorldApplication(Solution Solution) : base(Solution)
    {
        Name = "HelloWorld";
        Language = Language.CPlusPlus;
        Dialect = Dialect.CPP20;
        Flags = ProjectFlags.MultiProcessorCompile;
        Location = Path.Combine(Solution.Location, Name);
        TargetDirectory = Location;
        TargetName = "HelloWorld";
        Files.Add(@"Source\HelloWorld.cpp");
    }
}