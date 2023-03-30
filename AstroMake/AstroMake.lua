project "AstroMake"
	kind "SharedLib"
	language "C#"
	dotnetframework "4.8"
	csversion "7.3"
	targetdir "Binaries/%{prj.location}/%{cfg.buildcfg}"
	objdir "Binaries-Intermediate/%{prj.location}/%{cfg.buildcfg}"

	files {
		"Source/**.cs",
		"Properties/AssemblyInfo.cs",
	}

	
