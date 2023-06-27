using AstroMake;
using System;
using System.IO;

[Build]
public class Application : WindowedApplication
{
	public Application(Solution Solution) : base(Solution)
	{
		Name = "Application";
		Language = Language.CPlusPlus;
		Flags = ApplicationFlags.MultiProcessorCompile;
		TargetDirectory = Path.Combine(Solution.TargetDirectory, Name);
		
		Files.AddRange(new []
		{
			"Source/**.cpp",
			"Source/**.h"
		});
	}
}