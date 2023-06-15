
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
    public static int ToInt(this Error e)
    {
        return (int)e;
    }
}