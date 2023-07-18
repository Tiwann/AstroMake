group "HydroFramework"
project "glad"
	kind "StaticLib"
	language "C"
	staticruntime "On"
	flags "MultiProcessorCompile"

	targetdir(binaries)
	objdir(intermediate)

	files {
		"include/glad/gl.h",
		"include/KHR/khrplatform.h",
		"src/gl.c",
		"glad.lua"
	}

	includedirs "include"

	filter "Configurations:Debug"
		runtime "Debug"
		optimize "Off"
		symbols "On"

	filter "Configurations:Release"
		runtime "Release"
		optimize "On"
		symbols "Off"

