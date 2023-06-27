using AstroMake;
using System;
using System.IO;

[Build]
public class GLAD : StaticLibrary
{
	public GLAD(Solution Solution) : base(Solution)
	{
		Name = "glad";
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