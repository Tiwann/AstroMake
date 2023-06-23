using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AstroMake;

public class ArgumentParser
{
    private IEnumerable<string> Arguments { get; }
    public ArgumentParserSettings Settings { get; }


    private Dictionary<CommandLineOption, List<object>> ParsedArguments;
    private Collection<CommandLineOption> Options = new();
    private readonly IEnumerable<char> PrefixCharacters;

    public ArgumentParser(IEnumerable<string> Arguments, ArgumentParserSettings Settings)
    {
        this.Settings = Settings;
        this.Arguments = Arguments;
        ParsedArguments = new();
        PrefixCharacters = Settings.ShortFormatPrefix.ToCharArray().Union(Settings.LongFormatPrefix.ToCharArray());
    }
    
    public bool IsArgumentValid(string Argument)
    {
        var ShortNames = Options.Select(O => O.ShortName);
        var LongNames = Options.Select(O => O.LongName);
        string OptionName = GetOptionNameFromArgument(Argument);
        return Settings.Regex.IsMatch(Argument) && (ShortNames.Contains(OptionName[0]) || LongNames.Contains(OptionName));
    }

    private string GetOptionNameFromArgument(string Argument)
    {
        int AssignmentPosition = Argument.IndexOf(Settings.AssigmentCharacter);
        string TrimedArgument = Argument.TrimStart(PrefixCharacters.ToArray());
        return AssignmentPosition == -1 ? TrimedArgument : TrimedArgument.Substring(0, AssignmentPosition - 1);
    }

    private (string, object) SplitArgument(string Argument)
    {
        string Key;
        object Value;
        int AssignmentPosition = Argument.IndexOf(Settings.AssigmentCharacter);
        if (AssignmentPosition == -1)
        {
            Key = Argument.TrimStart(PrefixCharacters.ToArray());
            Value = true;
        }
        else
        {
            string TrimedKey = Argument.TrimStart(PrefixCharacters.ToArray());
            Key = TrimedKey.Substring(0, AssignmentPosition - 1);
            Value = Argument.Substring(AssignmentPosition + 1);
        }

        return (Key, Value);
    }

    public void AddOption(CommandLineOption Option)
    {
        Options.Add(Option);
    }
    
    public void AddOptions(Collection<CommandLineOption> InOptions)
    {
        Options = InOptions;
    }

    public void Parse()
    {
        foreach (string Argument in Arguments)
        {
            // Throw if argument isn't valid
            if (!IsArgumentValid(Argument)) throw new InvalidCommandLineArgumentException($"Argument \"{Argument}\" is not valid.");
            
            (string, object) SplittedArgument = SplitArgument(Argument);
            CommandLineOption Option = Options.Single(O => O.ShortName == SplittedArgument.Item1[0] || O.LongName == SplittedArgument.Item1);
            if (ParsedArguments.ContainsKey(Option) && !Option.AllowMultiple)
                throw new InvalidCommandLineArgumentException($"Cannot use argument \"{Argument}\" multiple times.");

            if(!ParsedArguments.ContainsKey(Option))
                ParsedArguments[Option] = new List<object>();
            ParsedArguments[Option].Add(SplittedArgument.Item2);
        }
    }
    
    
}
