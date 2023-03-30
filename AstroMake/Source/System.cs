using System;

namespace AstroMake 
{
    [Flags]
    public enum Systems : byte
    {
        None,
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
