﻿using System;

namespace AstroMake;


public class AstroException : Exception
{
    public Error Error { get; protected set; }
    public AstroException()
    {
    }
}


public class BadArgumentUsageException : AstroException
{
    public BadArgumentUsageException()
    {
        Error = Error.BadArgumentsUsage;
    }
}

public class NoArgumentProvidedException : AstroException
{
    public NoArgumentProvidedException()
    {
        Error = Error.NoArgumentsProvided;
    }
}

