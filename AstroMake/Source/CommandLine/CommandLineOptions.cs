namespace AstroMake;

public class CommandLineOption
{
    public char ShortName { get; }
    public string LongName { get; }
    public bool Required { get; }
    public bool AllowMultiple { get; }
    
    public CommandLineOption(char ShortName, string LongName, bool Required, bool AllowMultiple)
    {
        this.ShortName = ShortName;
        this.LongName = LongName;
        this.Required = Required;
        this.AllowMultiple = AllowMultiple;
    }
}
