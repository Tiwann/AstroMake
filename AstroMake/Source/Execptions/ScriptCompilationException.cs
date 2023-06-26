using System;

namespace AstroMake;

public class ScriptCompilationException : Exception
{
    public ScriptCompilationException(string Message) : base(Message)
    {
        
    }
}