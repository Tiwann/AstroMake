using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AstroMake;

public struct ArgumentParserSettings
{
    public readonly String ShortArgumentPrefix;
    public readonly String LongArgumentPrefix;

    public ArgumentParserSettings(String ShortArgumentPrefix, String LongArgumentPrefix)
    {
        this.ShortArgumentPrefix = ShortArgumentPrefix;
        this.LongArgumentPrefix = LongArgumentPrefix;
    }

    public static ArgumentParserSettings Default = new ArgumentParserSettings("/", "/");
    public static ArgumentParserSettings UnixStyle = new ArgumentParserSettings("-", "--");
}

public class ArgumentParser<T> where T : new()
{
    private readonly List<String> arguments;
    private readonly ArgumentParserSettings settings;
    private List<CommandLineOptionAttribute> attributes;

    public ArgumentParser(IEnumerable<String> Arguments)
    {
        arguments = Arguments.ToList();
        settings = ArgumentParserSettings.Default;
        attributes = new List<CommandLineOptionAttribute>();
    }

    public ArgumentParser(IEnumerable<String> Arguments, ArgumentParserSettings Settings)
    {
        arguments = Arguments.ToList();
        settings = Settings;
        attributes = new List<CommandLineOptionAttribute>();
    }

    public void Parse(Action<T> action)
    {
        PropertyInfo[] Properties = typeof(T).GetProperties();
        foreach (PropertyInfo Property in Properties)
        {
            if (Property.GetCustomAttribute(typeof(CommandLineOptionAttribute)) == null)
            {
                Log.Error($"In Class '{typeof(T).Name}': No {nameof(CommandLineOptionAttribute)} found on property {Property.Name}!");
                continue;
            }
            CommandLineOptionAttribute OptionAttribute = Property.GetCustomAttribute(typeof(CommandLineOptionAttribute)) as CommandLineOptionAttribute;
            attributes.Add(OptionAttribute);
        }
        
        var requiredOptionsAttributes = attributes.Where(att => att.Required);

        foreach (var requiredOption in requiredOptionsAttributes)
        {
            if (!(arguments.Contains($"/{requiredOption.ShortName}") || arguments.Contains($"/{requiredOption.LongName}")))
            {
                throw new BadArgumentUsageException();
            }
        }

        action.Invoke(new T());

        foreach (String Argument in arguments)
        {
            if(Argument.Equals($"/{Properties..ShortName}"))
        }
        
        
    }

}
