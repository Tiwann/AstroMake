using System;

namespace AstroMake;

[Flags]
public enum ApplicationFlags
{
    None = 0,
    MultiProcessorCompile = 1 << 0,
        
}