using AstroMake;
using System.IO;

[Build]
public class TestSolution : Solution
{
    public TestSolution()
    {
        Name = "TestWorkspace";
        Configurations.AddRange(new []{ new Configuration("Debug"), new Configuration("Release") });
        Architectures.AddRange(new [] { Architecture.x64, Architecture.x86});
        Systems.AddRange(new [] { AstroMake.System.Windows, AstroMake.System.Unix, AstroMake.System.MacOS });
		Platforms.AddRange(new [] { "OpenGL", "Vulkan", "DirectX" });
        Applications.Add("TestApplication");
        TargetDirectory = Path.Combine(Directory.GetCurrentDirectory(), Name);
    }
}

[Build]
public class TestApplication : ConsoleApplication
{
    public TestApplication(Solution Solution) : base(Solution)
    {
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