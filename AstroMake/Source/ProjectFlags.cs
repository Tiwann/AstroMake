using System;

namespace AstroMake;

[Flags]
public enum ProjectFlags
{
    None = 0,
    MultiProcessorCompile = 1 << 0,
}