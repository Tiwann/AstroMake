using System;
using System.Linq;

namespace AstroMake;

public static class StringExtensions
{
    public static Boolean ToBool(this string value)
    {
        string[] TrueValues = { "true", "True", "TRUE", "1" };
        string[] FalseValues = { "false", "False", "FALSE", "0" };

        if (TrueValues.Any(val => val.Equals(value))) return true;
        if (FalseValues.Any(val => val.Equals(value))) return false;
        return false;
    }

    public static string FindAndTrimEnd(this string str, string sequence)
    {
        int SequencePosition = str.IndexOf(sequence, StringComparison.Ordinal);
        if (SequencePosition == -1) return string.Empty;

        return str.Substring(0, SequencePosition);
    }
    
    public static string FindAndTrimStart(this string str, string sequence)
    {
        int SequencePosition = str.IndexOf(sequence, StringComparison.Ordinal);
        if (SequencePosition == -1) return string.Empty;

        return str.Substring(SequencePosition);
    }
    
}