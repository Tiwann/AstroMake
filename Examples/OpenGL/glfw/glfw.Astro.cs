using AstroMake;
using System;
using System.IO;


[Build]
public class GLFW : StaticLibrary
{
	public GLFW(Solution Solution) : base(Solution)
	{
		Name = "glfw";
		Language = Language.C;
		Flags = ApplicationFlags.MultiProcessorCompile;
		TargetDirectory = Path.Combine(Solution.TargetDirectory, Name);
		
		Files.AddRange(new []
		{
			"Source/**.c",
			"Source/**.h"
		});
	}
}