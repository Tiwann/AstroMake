using AstroMake;

public sealed class TestWorkspace : Workspace
{
    public TestWorkspace()
    {
        Name = "Test";
        Configurations = Configuration.CreateConfigurations("Debug", "Release");
        Architectures = Architectures.x64 | Architectures.x32;
        Systems = Systems.Windows | Systems.Unix | Systems.MacOS;
        Applications.Add("TestApplication");
    }
}

public sealed class TestApplication : ConsoleApplication
{
    public TestApplication()
    {
        Workspace = "TestWorkspace";
        Name = "TestApplication";
        Language = Language.CPlusPlus;
        Flags = ApplicationFlags.MultiProcessorCompile;
        
        Files.AddRange(new []
        {
            "Source/**.cpp",
            "Source/**.h"
        });
        
        IncludeDirectories.Add("opengl/include");
        Defines.Add("_CRT_SECURE_NO_WARNINGS");
        
    }
}