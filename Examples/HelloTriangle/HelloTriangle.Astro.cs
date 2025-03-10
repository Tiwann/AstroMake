using AstroMake;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[Build]
public class HelloTriangleSolution : Solution
{
    private readonly Configuration Debug = new Configuration("Debug", RuntimeType.Debug, ConfigurationFlags.None);
    private readonly Configuration Release = new Configuration("Release", RuntimeType.Release, ConfigurationFlags.None);
    
    public HelloTriangleSolution()
    {
        Name = "HelloTriangle";
        AddConfigurations(Debug, Release);
        Architecture = Architecture.x64;
        ProjectNames.Add("HelloTriangle");
        ProjectNames.Add("glad");
        ProjectNames.Add("glfw");
        TargetDirectory = Location;
    }
}