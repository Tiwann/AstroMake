project "AstroMake"
	kind "SharedLib"
	language "C#"
	dotnetframework "4.8.1"
	csversion "Latest"
	targetdir "Binaries/%{prj.location}/%{cfg.buildcfg}"
	objdir "Binaries-Intermediate/%{prj.location}/%{cfg.buildcfg}"

	files {
		"Source/**.cs",
		"Properties/AssemblyInfo.cs",
		"AstroMake.lua"
	}

	
