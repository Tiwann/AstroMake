using AstroMake;
using System.IO;

[Build]
public class gladLib : StaticLibrary
{
    public gladLib(Solution Solution) : base(Solution)
    {
        Name = "glad";
        Language = Language.C;
        Flags = ProjectFlags.MultiProcessorCompile;
        CStandard = CStandard.None;
        Location = Path.Combine(Solution.Location, "Vendor", Name);
        TargetDirectory = Location;
        TargetName = Name;
        Files.Add(@"src\**.c");
        Files.Add(@"include\**.h");
        IncludeDirectories.Add(Path.Combine(Location, "include"));
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