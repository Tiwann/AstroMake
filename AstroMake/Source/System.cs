using System;

namespace AstroMake;

[Flags]
public enum System
{
    None = 0,
    Windows = 1 << 0,
    Unix = 1 << 1,
    MacOS = 1 << 2,
    Android = 1 << 3,
    IOS = 1 << 4,
    XboxSeries = 1 << 5,
    PS5 = 1 << 6,
    Switch = 1 << 7
}