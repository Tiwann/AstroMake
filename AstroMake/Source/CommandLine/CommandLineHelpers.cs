using System;
using System.Collections.Generic;

namespace AstroMake;

public static class DictionaryHelpers
{
    public static void Add<T, U>(this Dictionary<T, U> Dictionary, (T, U) Tuple)
    {
        Dictionary.Add(Tuple.Item1, Tuple.Item2);
    }

    public static void Add<T, U>(this Dictionary<T, U> Dictionary, Tuple<T, U> Tuple)
    {
        Dictionary.Add(Tuple.Item1, Tuple.Item2);
    }
}