using System;

namespace AstroMake;

public static class Externders
{
    public static bool IsSubclassOf<T>(this Type Type)
    {
        return Type.IsSubclassOf(typeof(T));
    }

    public static T CreateInstance<T>(this Type Type, params dynamic[] Parameters) where T : class
    {
        return Activator.CreateInstance(Type, Parameters) as T;
    }
}