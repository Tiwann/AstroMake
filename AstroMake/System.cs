using System;

namespace AstroMake 
{
    [Flags]
    public enum System : byte
    {
        Windows,
        Unix,
        MacOS,
        Android,
        IOS,
        XboxSeries,
        PS5,
        Switch
    }
}
