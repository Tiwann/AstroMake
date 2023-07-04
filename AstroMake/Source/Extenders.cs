using System;
using System.Collections.Generic;
using System.Linq;

namespace AstroMake;

public static class Extenders
{
    public static bool IsSubclassOf<T>(this Type Type)
    {
        return Type.IsSubclassOf(typeof(T));
    }

    public static T CreateInstance<T>(this Type Type, params dynamic[] Parameters) where T : class
    {
        return Type.IsSubclassOf<T>() ? Activator.CreateInstance(Type, Parameters) as T : null;
    }

    public static T Last<T>(this IEnumerable<T> List) where T : class
    {
        var Temp = List.ToList();
        return Temp.Count != 0 ? Temp[Temp.Count - 1] : null;
    }

    public static bool IsEmpty<T>(this IEnumerable<T> List)
    {
        return List.Count() == 0;
    }
}