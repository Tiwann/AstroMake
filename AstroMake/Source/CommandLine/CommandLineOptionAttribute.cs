using System;

namespace AstroMake;

[AttributeUsage(AttributeTargets.Property)]
public class CommandLineOptionAttribute : Attribute
{
    public Char ShortName { get; }
    public String LongName { get; }
    public String HelpText { get; }
    public bool Required { get; }


    public CommandLineOptionAttribute(Char ShortName, String LongName, String HelpText, bool Required)
    {
        this.ShortName = ShortName;
        this.LongName = LongName;
        this.HelpText = HelpText;
        this.Required = Required;
    }
}