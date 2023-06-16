using System;

namespace AstroMake;


/// <summary>
/// Attribute the put on class property members. Specifies how the parser should handle it.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class CommandLineOptionAttribute : Attribute
{
    /// <summary>
    /// Option short name.
    /// </summary>
    public Char ShortName { get; }
    
    /// <summary>
    /// Option long name
    /// </summary>
    public String LongName { get; }

    /// <summary>
    /// Help text to display
    /// </summary>
    public String HelpText { get; }
    
    /// <summary>
    /// Is this option required
    /// </summary>
    public bool Required { get; }


    public CommandLineOptionAttribute(Char ShortName, String LongName, String HelpText, bool Required)
    {
        this.ShortName = ShortName;
        this.LongName = LongName;
        this.HelpText = HelpText;
        this.Required = Required;
    }
}