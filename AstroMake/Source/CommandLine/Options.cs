namespace AstroMake;

public class Options
{
    public enum BuildType
    {
        None,
        VisualStudio,
        XCode,
        Makefile
    }
        
    [CommandLineOption(ShortName: 'h', LongName: "help", "Show help", false)]
    public bool Help { get; set; }
        
    [CommandLineOption(ShortName: 'b', LongName: "build", "Generate using Astro Make scripts", true)]
    public BuildType Type { get; set; }


    public Options()
    {
        Help = false;
        Type = BuildType.None;
    }
        
    public Options(bool Help)
    {
        this.Help = Help;
        Type = BuildType.None;
    }
        
    public Options(bool Help, BuildType Type)
    {
        this.Help = Help;
        this.Type = Type;
    }
        
}