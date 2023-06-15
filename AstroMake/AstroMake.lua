project "AstroMake"
	kind "ConsoleApp"
	language "C#"
	dotnetframework "4.8.1"
	csversion "Latest"
	targetdir "%{wks.location}/Binaries/%{prj.name}/%{cfg.buildcfg}"
	objdir "%{wks.location}/Binaries-Intermediate/%{prj.name}/%{cfg.buildcfg}"

	files {
		"Source/**.cs",
		"Properties/AssemblyInfo.cs",
		"AstroMake.lua"
	}
	
	links {
	    "Microsoft.CSharp",
	    "System",
	    "System.Xml"
	}
	
	filter "Configurations:Debug"
	    runtime "Debug"
	    optimize "Off"
	    symbols "On"
	    
	filter "Configurations:Release"
    	    runtime "Release"
    	    optimize "On"
    	    symbols "Off"
    	    
    	    
    	    

	
