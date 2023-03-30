using System;

namespace AstroMake 
{
    [Flags]
    public enum Architectures : byte
    {
        x86 = 0,
        x64 = 1,
        ARM = 2,
        ARM64 = 3,
        x32 = x86,
        x86_32 = x86,
        amd64 = x64,
        x86_64 = x64
    }
}
