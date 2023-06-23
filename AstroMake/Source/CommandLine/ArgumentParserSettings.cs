﻿using System.Text.RegularExpressions;


namespace AstroMake;


public class ArgumentParserSettings
{
    public readonly string ShortFormatPrefix;
    public readonly string LongFormatPrefix;
    public readonly char AssigmentCharacter;

    public Regex Regex => new(@$"^({ShortFormatPrefix}|{LongFormatPrefix})[A-Za-z]+({AssigmentCharacter}[A-Za-z0-9:/\\]+)?$");

    public ArgumentParserSettings(string Short, string Long, char Assignment)
    {
        ShortFormatPrefix = Short;
        LongFormatPrefix = Long;
        AssigmentCharacter = Assignment;
    }
    
    public static readonly ArgumentParserSettings WindowsStyle = new("/", "/", ':');
    public static readonly ArgumentParserSettings UnixStyle = new("-", "--", '=');
}