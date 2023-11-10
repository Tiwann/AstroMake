using AstroMake;
using System.IO;

[Build]
public class HelloTriangleApp : ConsoleApplication
{
    public HelloTriangleApp(Solution Solution) : base(Solution)
    {
        Name = "HelloTriangle";
        Language = Language.CPlusPlus;
        CppStandard = CPPStandard.CPP20;
        Flags = ProjectFlags.MultiProcessorCompile;
        Location = Path.Combine(Solution.Location, Name);
        TargetDirectory = Location;
        TargetName = Name;
        Files.Add(@"Source\**.h");
        Files.Add(@"Source\**.cpp");
        IncludeDirectories.AddRange(new[]
        {
            Path.Combine(Solution.Location, "Vendor", "glad", "include"),
            Path.Combine(Solution.Location, "Vendor", "glfw", "include"),
        });
        Defines.AddRange(new [] { "_CRT_SECURE_NO_WARNINGS" });
        Links.Add("glad");
        Links.Add("glfw");
    }

    public override void Configure(Configuration Configuration)
    {
        if (Configuration.Name == "Debug")
        {
            BinariesDirectory = Path.Combine(Solution.Location, "Binaries", "Debug", Name);
            IntermediateDirectory = Path.Combine(Solution.Location, "Intermediate", "Debug", Name);
            
            return;
        }

        if (Configuration.Name == "Release")
        {
            BinariesDirectory = Path.Combine(Solution.Location, "Binaries", "Release", Name);
            IntermediateDirectory = Path.Combine(Solution.Location, "Intermediate", "Release", Name);
            
            return;
        }
    }
}