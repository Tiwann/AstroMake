namespace AstroMake;


/// <summary>
/// Contains all the available <see cref="CommandLineOption"/> for the program
/// All options are static so we can access it all through the program 
/// </summary>
public static class Options
{
    public static readonly CommandLineOption Help;
    public static readonly CommandLineOption Build;
    public static readonly CommandLineOption RootDir;
    public static readonly CommandLineOption Source;
    public static readonly CommandLineOption Test;

    static Options()
    {
        Help = new CommandLineOption('h', "help", false, false, "Show help");
        Build = new CommandLineOption('b', "build", true, false, "Generate using Astro Make build scripts",
            new("targets",
            new("vstudio",  "Generate Visual Studio Solution (latest version)"),
            new("makefile", "Generate Makefiles"),
            new("xcode",    "Generate XCode Project")));
        RootDir = new CommandLineOption('d', "dir", false, false, "Specify a build script search root directory");
        Source = new CommandLineOption('s', "source", false, true, "Add specific build script to the build queue");
        Test = new CommandLineOption('a', "arg", true, false, "Test option",
            new("commands", 
            new("cmd", "Option command"), 
            new("pwrsh", "Option powershell")));
    }
}