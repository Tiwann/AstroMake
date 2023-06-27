using AstroMake;
using System;
using System.IO;

[Build]
public class OpenGLSolution : Solution
{
	public OpenGLSolution()
	{
		Name = "OpenGL";
		Architecture = Architecture.x64;
		Configurations.AddRange(new []{ new Configuration("Debug"), new Configuration("Release") });
		Platforms.AddRange(new[]{ "OpenGL", "Vulkan" });
		Systems.AddRange(new[] { AstroMake.System.Windows, AstroMake.System.Unix });
		ApplicationNames.AddRange(new[] { "Application", "glfw", "glad" });
		TargetDirectory = Path.Combine(Directory.GetCurrentDirectory());
	}
}