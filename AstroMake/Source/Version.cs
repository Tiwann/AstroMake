﻿using System;

namespace AstroMake;
public class Version : Object, IComparable<Version>
{
    public ushort Major { get; }
    public ushort Minor { get; }

    public Version(ushort Maj, ushort Min)
    {
        Major = Maj;
        Minor = Min;
    }

    public int CompareTo(Version Other)
    {
        int Result = 0;
        
        if (Major > Other.Major)
            Result = 1;
        if (Major == Other.Major && Minor > Other.Minor)
            Result = 1;
        if (Major == Other.Major && Minor == Other.Minor)
            Result = 0;
        if (Major < Other.Major)
            Result = -1;
        if (Major == Other.Major && Minor < Other.Minor)
            Result = -1;

        return Result;
    }


    public override String ToString()
    {
        return $"{Major}.{Minor}";
    }

    public static Version AstroVersion = new (1, 0);
}
