using System;

namespace AstroMake;

public static class Log
{
    public static void Trace(string Message)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{Message}");
        Console.ResetColor();
    }

    public static void Success(string Message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{Message}");
        Console.ResetColor();
    }
    
    public static void Warning(string Message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{Message}");
        Console.ResetColor();
    }
    
    public static void Error(string Message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{Message}");
        Console.ResetColor();
    }
}