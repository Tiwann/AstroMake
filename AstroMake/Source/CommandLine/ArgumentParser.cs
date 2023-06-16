using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AstroMake;


/// <summary>
/// Template class that parses command line arguments into an object of class Class
/// </summary>
/// <typeparam name="Class">The class to fill atrguments info to</typeparam>
public class ArgumentParser<Class> where Class : new()
{
    /// <summary>
    /// A copy of the arguments
    /// </summary>
    private readonly List<String> arguments;
    
    /// <summary>
    /// Parser settings
    /// <see cref="ArgumentParserSettings"/>
    /// </summary>
    private readonly ArgumentParserSettings settings;

    public ArgumentParser(IEnumerable<String> Arguments)
    {
        arguments = Arguments.ToList();
        settings = ArgumentParserSettings.Default;
    }

    public ArgumentParser(IEnumerable<String> Arguments, ArgumentParserSettings Settings)
    {
        arguments = Arguments.ToList();
        settings = Settings;
    }

    /// <summary>
    /// Parses the arguments
    /// </summary>
    /// <param name="action">Function that will be called with the parsed object. Give the user the possility to handle the parsed object's properties</param>
    /// <exception cref="NoArgumentProvidedException">No arguments were found and AllowNoArguments is false <see cref="ArgumentParserSettings"/></exception>
    /// <exception cref="BadArgumentUsageException">A required option was not found int the arguments list</exception>
    /// <exception cref="BadArgumentUsageException">The arguments don't have the right formats</exception>
    public void Parse(Action<Class> action)
    {
        if (!settings.AllowNoArguments)
        {
            if (arguments.Count == 0)
            {
                throw new NoArgumentProvidedException();
            }
        }
        
        Type ParsedObjectType = typeof(Class);
        PropertyInfo[] Properties = ParsedObjectType.GetProperties();
        List<CommandLineOptionAttribute> Attributes = new();

        // Check if all properties have a CommandLineOption attribute
        foreach (PropertyInfo Property in Properties)
        {
            if (Property.GetCustomAttribute(typeof(CommandLineOptionAttribute)) == null)
            {
                Log.Error($"In Class '{typeof(Class).Name}': No {nameof(CommandLineOptionAttribute)} found on property {Property.Name}!");
                continue;
            }
            CommandLineOptionAttribute OptionAttribute = Property.GetCustomAttribute<CommandLineOptionAttribute>();
            Attributes.Add(OptionAttribute);
        }
        
        // Throw a bad usage exception if one of required arguments are not found
        var RequiredOptionsAttributes = Attributes.Where(att => att.Required);
        if (RequiredOptionsAttributes.Any(RequiredOption => arguments.Contains($"{settings.ShortArgumentPrefix}{RequiredOption.ShortName}") || arguments.Contains($"{settings.LongArgumentPrefix}{RequiredOption.LongName}")))
        {
            throw new BadArgumentUsageException();
        }

        
        Class ParsedObject = new();
        
        IEnumerable<String> ShortNames = Attributes.Select(Attribute => Attribute.ShortName.ToString());
        IEnumerable<String> LongNames = Attributes.Select(Attribute => Attribute.LongName);
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
