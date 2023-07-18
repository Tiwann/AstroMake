using AstroMake;
using System.IO;

[Build]
public class glfwLib : StaticLibrary
{
    public glfwLib(Solution Solution) : base(Solution)
    {
        Name = "glfw";
        Language = Language.C;
        Flags = ProjectFlags.MultiProcessorCompile;
        CStandard = CStandard.None;
        Location = Path.Combine(Solution.Location, "Vendor", Name);
        TargetDirectory = Location;
        TargetName = Name;
        Files.AddRange(new []
        {
            "include/GLFW/glfw3.h",
            "include/GLFW/glfw3native.h",
            "src/internal.h",
            "src/platform.h",
            "src/mappings.h",
            "src/mappings.h.in",
            "src/context.c",
            "src/init.c",
            "src/input.c",
            "src/monitor.c",
            "src/platform.c",
            "src/platform.h",
            "src/vulkan.c",
            "src/window.c",
            "src/egl_context.c",
            "src/osmesa_context.c",
            "src/null_platform.h",
            "src/null_joystick.h",
            "src/null_joystick.c",
            "src/null_init.c",
            "src/null_monitor.c",
            "src/null_window.c",
            "src/win32_init.c",
            "src/win32_module.c",
            "src/win32_joystick.c",
            "src/win32_joystick.h",
            "src/win32_platform.h",
            "src/win32_monitor.c",
            "src/win32_time.h",
            "src/win32_time.c",
            "src/win32_thread.h",
            "src/win32_thread.c",
            "src/win32_window.c",
            "src/wgl_context.c"
        });
        
        IncludeDirectories.Add(Path.Combine(Location, "include"));
        
        Defines.AddRange(new []
        {
            "_GLFW_WIN32",
            "_CRT_SECURE_NO_WARNINGS"
        });
    }

    public override void Configure(Configuration Configuration)
    {
        if (Configuration.Name == "Debug")
        {
            BinariesDirectory = Path.Combine(Solution.Location, "Binaries", "Debug", Name);
            IntermediateDirectory = Path.Combine(Solution.Location, "Intermediate", "Debug", Name);
            return;
        }

        if (Configuration.Name == "Release")
        {
            BinariesDirectory = Path.Combine(Solution.Location, "Binaries", "Release");
            IntermediateDirectory = Path.Combine(Solution.Location, "Intermediate", "Release", Name);
        }
    }
}