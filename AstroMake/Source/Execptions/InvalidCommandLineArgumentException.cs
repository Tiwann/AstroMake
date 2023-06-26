using System;

namespace AstroMake;
public class InvalidCommandLineArgumentException : Exception
{
    //public override string Message => "Invalid argument format or usage";

    public InvalidCommandLineArgumentException(string message) : base(message)
    {
        
    }
}