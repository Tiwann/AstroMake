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
    
}