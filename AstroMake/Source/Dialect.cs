using System;

namespace AstroMake;

public enum Dialect
{
    CPP20,
    CPP17,
    CPP14,
    CPP11,
    CPPLatest,
    CSharp8,
    CSharp9,
    CSharp10,
    CSharp11,
    CSharpLatest
}

public static class DialectHelpers
{
    public static bool IsCPP(this Dialect Dialect)
    {
        return Dialect is Dialect.CPP11 or Dialect.CPP14 or Dialect.CPP17 or Dialect.CPP20 or Dialect.CPPLatest;
    }

    public static string GetString(this Dialect Dialect)
    {
        switch (Dialect)
        {
            case Dialect.CPP20:
                return "C++20";
            case Dialect.CPP17:
                return "C++17";
            case Dialect.CPP14:
                return "C++14";
            case Dialect.CPP11:
                return "C++11";
            case Dialect.CPPLatest:
                return "C++ Latest";
            case Dialect.CSharp8:
                return "C#8";
            case Dialect.CSharp9:
                return "C#9";
            case Dialect.CSharp10:
                return "C#10";
            case Dialect.CSharp11:
                return "C#11";
            case Dialect.CSharpLatest:
                return "C# Latest";
            default:
                throw new ArgumentOutOfRangeException(nameof(Dialect), Dialect, null);
        }
    }
}