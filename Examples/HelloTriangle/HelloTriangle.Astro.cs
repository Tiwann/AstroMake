using AstroMake;

[Build]
public class HelloTriangleSolution : Solution
{
    private readonly Configuration Debug = new Configuration("Debug", RuntimeType.Debug, ConfigurationFlags.None);
    private readonly Configuration Release = new Configuration("Release", RuntimeType.Release, ConfigurationFlags.None);
    
    public HelloTriangleSolution()
    {
        Name = "HelloTriangle";
        Architecture = Architecture.x64;
        AddConfigurations(Debug, Release);
        ProjectNames.AddRange(new [] { "HelloTriangle", "glad", "glfw" });
    }
}

