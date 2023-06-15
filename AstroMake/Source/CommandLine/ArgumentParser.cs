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
    private readonly List<CommandLineOptionAttribute> attributes;
    private T ParsedObject;

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
        if (arguments.Count == 0)
        {
            throw new NoArgumentProvidedException();
        }
        
        Type ParsedObjectType = typeof(T);
        PropertyInfo[] Properties = ParsedObjectType.GetProperties();
        foreach (PropertyInfo Property in Properties)
        {
            if (Property.GetCustomAttribute(typeof(CommandLineOptionAttribute)) == null)
            {
                Log.Error($"In Class '{typeof(T).Name}': No {nameof(CommandLineOptionAttribute)} found on property {Property.Name}!");
                continue;
            }
            CommandLineOptionAttribute OptionAttribute = Property.GetCustomAttribute<CommandLineOptionAttribute>();
            attributes.Add(OptionAttribute);
        }
        
        var RequiredOptionsAttributes = attributes.Where(att => att.Required);

        /*if (RequiredOptionsAttributes.Any(RequiredOption => !(arguments.Contains($"/{RequiredOption.ShortName}") || arguments.Contains($"/{RequiredOption.LongName}"))))
        {
            throw new BadArgumentUsageException();
        }*/

        
        ParsedObject = new T();
        
        IEnumerable<String> ShortNames = attributes.Select(Attribute => Attribute.ShortName.ToString());
        IEnumerable<String> LongNames = attributes.Select(Attribute => Attribute.LongName);
        List<String> Names = ShortNames.Union(LongNames).ToList();

        for (int Index = 0; Index < arguments.Count; Index++)
        {
            String Argument = arguments[Index].Remove(0, 1);
            if (Names.Contains(Argument))
            {
                var PropertyName = Properties.Where(p =>
                {
                    CommandLineOptionAttribute Att = p.GetCustomAttribute<CommandLineOptionAttribute>();
                    bool Check = Att.ShortName.ToString() == Argument || Att.LongName == Argument;
                    return Check;
                }).Select(p => p.Name);

                PropertyInfo Info = ParsedObjectType.GetProperty(PropertyName.Single());


                if (Info != null && Info.CanWrite)
                {
                    if (Info.PropertyType == typeof(Boolean))
                    {
                        Info.SetValue(ParsedObject, true);
                    }
                }
            }
        }


        action.Invoke(ParsedObject);
    }

}
