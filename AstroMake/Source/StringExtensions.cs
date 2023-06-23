using System;
using System.Linq;

namespace AstroMake;

public static class StringExtensions
{
    public static Boolean ToBool(this String value)
    {
        String[] TrueValues = { "true", "True", "TRUE", "1" };
        String[] FalseValues = { "false", "False", "FALSE", "0" };

        return TrueValues.Any(val => val.Equals(value)) || FalseValues.Any(val => val.Equals(value));
    }
    
}