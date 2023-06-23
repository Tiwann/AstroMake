
using System;

namespace AstroMake;

public enum Error
{
    NoError = 0,
    Error = -1,
    NoBuildScriptFound = -2,
    CompileError = -3,
    NoArgumentsProvided = -4,
    BadArgumentsUsage = -5
}

public static class ErrorHelpers
{
    
    public static string ToStr(this Error e) 
    {
        switch (e)
        {
            case Error.Error:
                return "Error";
            case Error.NoError:
                return "Sucess";
            case Error.NoBuildScriptFound:
                return "No build scripts found";
            case Error.CompileError:
                return "Compile error";
            case Error.NoArgumentsProvided:
                return "No arguments provided";
            case Error.BadArgumentsUsage:
                return "Bad arugument usage";
            default:
                throw new ArgumentOutOfRangeException(nameof(e), e, null);
        }
    }
}