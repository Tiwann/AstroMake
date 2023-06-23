using System;

namespace AstroMake;

public class NoArgumentProvidedException : Exception
{
    public NoArgumentProvidedException(string Message) : base(Message)
    {
        
    }
}