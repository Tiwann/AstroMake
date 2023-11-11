using System;
using System.Reflection;

namespace AstroMake;
public class Version : IComparable<Version>
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
        
        if (Major > Other!.Major)
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


    public override string ToString()
    {
        return $"{Major}.{Minor}";
    }

    public static readonly Version AstroVersion = new (Convert.ToUInt16(Assembly.GetExecutingAssembly().GetName().Version!.Major), Convert.ToUInt16(Assembly.GetExecutingAssembly().GetName().Version!.Minor));
}

