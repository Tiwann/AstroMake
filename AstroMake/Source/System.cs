using System;

namespace AstroMake;

[Flags]
public enum System
{
    None = 0,
    Windows,
    Unix,
    MacOS,
    Android,
    IOS,
    XboxSeries,
    PS5,
    Switch
}