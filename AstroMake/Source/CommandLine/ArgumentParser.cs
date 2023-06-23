using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AstroMake;

public class ArgumentParser
{
    private IEnumerable<string> Arguments { get; }
    public ArgumentParserSettings Settings { get; }


    private readonly Dictionary<CommandLineOption, List<object>> ParsedArguments;
    private Collection<CommandLineOption> Options = new();
    private readonly IEnumerable<char> PrefixCharacters;

    public ArgumentParser(IEnumerable<string> Arguments, ArgumentParserSettings Settings)
    {
        this.Settings = Settings;
        this.Arguments = Arguments;
        ParsedArguments = new();
        PrefixCharacters = Settings.ShortFormatPrefix.ToCharArray().Union(Settings.LongFormatPrefix.ToCharArray());
    }
    
    private bool IsArgumentValid(string Argument)
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
    
    public void AddOptions(params CommandLineOption[] InOptions)
    {
        Options = new Collection<CommandLineOption>(InOptions);
    }

    public void Parse()
    {
        if (Arguments.Count() == 0)
            throw new NoArgumentProvidedException($"No arguments were provided. Try {Assembly.GetExecutingAssembly().GetName().Name} {Settings.LongFormatPrefix}help.");
        //TODO: Check if any required options was not found

        foreach (string Argument in Arguments)
        {
            // Throw if argument isn't valid
            if (!IsArgumentValid(Argument)) throw new InvalidCommandLineArgumentException($"Argument \"{Argument}\" is not valid.");
            
            (string, object) SplittedArgument = SplitArgument(Argument);
            // TODO: Try to patch this line which makes --h acceptable for help -h --help
            CommandLineOption Option = Options.Single(O => O.ShortName == SplittedArgument.Item1[0] || O.LongName == SplittedArgument.Item1);
            if (ParsedArguments.ContainsKey(Option) && !Option.AllowMultiple)
                throw new InvalidCommandLineArgumentException($"Cannot use argument \"{Argument}\" multiple times.");

            if(!ParsedArguments.ContainsKey(Option))
                ParsedArguments[Option] = new List<object>();
            ParsedArguments[Option].Add(SplittedArgument.Item2);
        }
    }

    public string GetHelpText()
    {
        StringBuilder Builder = new();
        Builder.AppendLine($"Usage: {Assembly.GetExecutingAssembly().GetName().Name} [options]");
        Builder.AppendLine("options:");
        foreach (CommandLineOption Option in Options)
        {
            string values = Option.PossibleValues != null ? $"[{Option.PossibleValues.Name}]" : string.Empty;
            string tabs = string.IsNullOrEmpty(values) ? "\t\t" : "\t";
            Builder.AppendLine($"    {Settings.ShortFormatPrefix}{Option.ShortName} {Settings.LongFormatPrefix}{Option.LongName} {values}{tabs}{Option.HelpText}");
        }

        foreach (CommandLineOption Option in Options)
        {
            if (Option.PossibleValues == null) continue;
            Builder.AppendLine($"{Option.PossibleValues.Name}:");
            foreach (CommandLineOptionPossibleValue PossibleValue in Option.PossibleValues.PossibleValues)
            {
                Builder.AppendLine($"    {PossibleValue.Name}\t{PossibleValue.HelpText}");
            }
        }
        return Builder.ToString();
    }
    
    public bool GetBool(CommandLineOption Option)
    {
        if (ParsedArguments.ContainsKey(Option))
        {
            var Value = ParsedArguments[Option][0];
            if (Value is bool BVal) return BVal;
            if (Value is string SVal) return SVal.ToBool();
        }
        return false;
    }

    public string GetString(CommandLineOption Option)
    {
        if (ParsedArguments.ContainsKey(Option))
        {
            var Value = ParsedArguments[Option][0];
            if (Value is bool BVal) return BVal.ToString();
            if (Value is string SVal) return SVal;
        }
        return string.Empty;
    }
}
