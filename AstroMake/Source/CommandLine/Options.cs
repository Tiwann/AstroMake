using System;

namespace AstroMake;

public class Options
{
    
    [CommandLineOption(ShortName: 'h', LongName: "help", "Show help", false)]
    public bool Help { get; set; }
        
    [CommandLineOption(ShortName: 'b', LongName: "build", "Generate using Astro Make scripts", true, PossibleValues = new []{ "vstudio", "makefile", "xcode" })]
    public String Type { get; set; }


    public Options()
    {
        Help = false;
    }
}