project "AstroMakeInstaller"
	kind "ConsoleApp"
	language "C#"
	dotnetframework "net7.0"
	csversion "Latest"
	targetdir "%{wks.location}/Binaries/%{prj.name}/%{cfg.buildcfg}"
	objdir "%{wks.location}/Binaries-Intermediate/%{prj.name}/%{cfg.buildcfg}"
	icon "%{wks.location}/Resources/AstroMakeLogo.ico"

	files {
		"Source/**.cs",
		"Properties/AssemblyInfo.cs",
		"AstroMakeInstaller.lua",
	}
	
	links {

	}
	
	filter "Configurations:Debug"
	    runtime "Debug"
	    optimize "Off"
	    symbols "On"
	    
	filter "Configurations:Release"
    	    runtime "Release"
    	    optimize "On"
    	    symbols "Off"
    	    
    	    
    	    
    	    

	
