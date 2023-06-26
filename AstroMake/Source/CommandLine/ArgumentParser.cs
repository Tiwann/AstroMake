using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AstroMake;

/// <summary>
/// Provides utilities to parse command line arguments
/// </summary>
public class ArgumentParser
{

    private readonly IEnumerable<string> Arguments;
    public ArgumentParserSettings Settings { get; }
    
    private readonly Dictionary<CommandLineOption, List<dynamic>> ParsedArguments;
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

    private (string, dynamic) SplitArgument(string Argument)
    {
        string Key;
        dynamic Value;
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

    public void AddOptions(params CommandLineOption[] InOptions)
    {
        Options = new Collection<CommandLineOption>(InOptions);
    }

    public void Parse()
    {
        if (Arguments.Count() == 0)
            throw new NoArgumentProvidedException($"No arguments were provided. Try {Assembly.GetExecutingAssembly().GetName().Name} {Settings.LongFormatPrefix}help.");
        
        bool AllRequiredOptionsFound = Arguments.Any(A =>
        {
            string OptionName = GetOptionNameFromArgument(A);
            var ShortNames = Options.Where(O => O.Required).Select(O => O.ShortName);
            var LongNames = Options.Where(O => O.Required).Select(O => O.LongName);
            return (ShortNames.Contains(OptionName[0]) && OptionName.Length <= 1) || LongNames.Contains(OptionName);
        });

        if (!AllRequiredOptionsFound)
        {
            throw new InvalidCommandLineArgumentException("A required option was not found.");
        }

        foreach (string Argument in Arguments)
        {
            // Throw if argument isn't valid
            if (!IsArgumentValid(Argument)) throw new InvalidCommandLineArgumentException($"Argument \"{Argument}\" is not valid.");
            
            (string, dynamic) SplittedArgument = SplitArgument(Argument);
            // TODO: Try to patch this line which makes --h acceptable for help -h --help
            CommandLineOption Option = Options.Single(O => O.ShortName == SplittedArgument.Item1[0] || O.LongName == SplittedArgument.Item1);
            if (ParsedArguments.ContainsKey(Option) && !Option.AllowMultiple)
                throw new InvalidCommandLineArgumentException($"Cannot use argument \"{Argument}\" multiple times.");

            if(!ParsedArguments.ContainsKey(Option))
                ParsedArguments[Option] = new List<dynamic>();
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
            string Values = Option.PossibleValues != null ? $"[{Option.PossibleValues.Name}]" : string.Empty;
            string Tabs = string.IsNullOrEmpty(Values) ? "\t\t" : "\t";
            string Required = Option.Required ? "(Required)" : string.Empty;
            Builder.AppendLine($"    {Settings.ShortFormatPrefix}{Option.ShortName} {Settings.LongFormatPrefix}{Option.LongName} {Values}{Tabs}{Option.HelpText} {Required}");
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

    public dynamic GetValue<T>(CommandLineOption Option)
    {
        if (ParsedArguments.ContainsKey(Option))
        {
            T Value = ParsedArguments[Option][0];
            return Value;
        }

        return false;
    }

    public IEnumerable<string> GetValues(CommandLineOption Option)
    {
        IEnumerable<string> Result = new string[] { };
        if (ParsedArguments.ContainsKey(Option))
        {
            ParsedArguments[Option].ForEach(S => Result.ToList().Add(S));
            return Result;
        }

        return null;
    }
}
