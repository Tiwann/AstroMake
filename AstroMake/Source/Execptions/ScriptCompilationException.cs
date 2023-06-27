using System;

namespace AstroMake;

public class ScriptCompilationFailedException : Exception
{
    public ScriptCompilationFailedException(string Message) : base(Message)
    {
        
    }
}