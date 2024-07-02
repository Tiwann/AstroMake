# <img src="/Resources/AstroMakeLogo.png" width="48" height="48" class="astro-logo" style=".astro-logo { text-align: justify; }"> Astro Make

## Introduction
Astro Make is a simple and fast build tool that generates
visual studio solutions and projects. <br>
It works like premake and cmake but it uses _C# scripts_ to describe a solution for generation. 
The idea is to keep the scripts writing and reading as _simple and fast_ as possible. <br>
Using C# helps for **syntax highlighting** and **code completion**. 
Astro Make updated to use .NET 7 and is now cross-platform. See releases for download.


## How it works
When running Astro Make program, it recursively looks for `.Astro.cs` files that will be _compiled_ 
into an assembly in runtime.
The compiler gives error messages so the users know if their code have mistakes.

## How to use
### Create a solution
Create an empty folder and create a `.Astro.cs` file that contains definition of a *`Solution`*.<br>
Don't forget to mark the class with a *`[Build]`* attribute.

```csharp
using AstroMake;

[Build]
public class HelloWorldSolution : Solution
{
    private readonly Configuration Debug = new Configuration("Debug", RuntimeType.Debug, ConfigurationFlags.None);
    private readonly Configuration Release = new Configuration("Release", RuntimeType.Release, ConfigurationFlags.None);
    
    public HelloWorldSolution()
    {
        Name = "HelloWorld";
        TargetDirectory = Location;
        AddConfigurations(Debug, Release);
        Architecture = Architecture.x64;
        
        // Add projects to solution by adding their Name
        ProjectNames.Add("HelloWorld");
        
    }
}
```

Then create ``.Astro.cs`` file that contains definition of the project

```csharp
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
```
 
