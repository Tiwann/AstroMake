using AstroMake;
using System;
using System.IO;

[Build]
public class OpenGLSolution : Solution
{
	public OpenGLSolution()
	{
		Name = "OpenGL";
		Architectures.Add(Architecture.x64);
		Configurations.AddRange(new []{ new Configuration("Debug"), new Configuration("Release") });
		Systems.AddRange(new[] { AstroMake.System.Windows, AstroMake.System.Unix });
		Applications.AddRange(new[] { "Application", "glfw", "glad" });
		TargetDirectory = Path.Combine(Directory.GetCurrentDirectory());
	}
}