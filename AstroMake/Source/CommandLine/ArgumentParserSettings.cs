using System;

namespace AstroMake;

/// <summary>
/// Set Argument Parser parameters such as argument formats
/// </summary>
public struct ArgumentParserSettings
{
    /// <summary>
    /// Characters for short format arguments
    /// </summary>
    public readonly String ShortArgumentPrefix;
    
    /// <summary>
    /// Characters for long format arguments
    /// </summary>
    public readonly String LongArgumentPrefix;


    /// <summary>
    /// Assignment character
    /// </summary>
    public readonly Char AssignmentCharacter;


    /// <summary>
    /// Tell the parser that the program could run without any arguemnts
    /// </summary>
    public Boolean AllowNoArguments = false;
    
    
    public ArgumentParserSettings(String ShortArgumentPrefix, String LongArgumentPrefix, Char AssignmentCharacter)
    {
        this.ShortArgumentPrefix = ShortArgumentPrefix;
        this.LongArgumentPrefix = LongArgumentPrefix;
        this.AssignmentCharacter = AssignmentCharacter;
    }

    public ArgumentParserSettings WithAllowNoArguments(Boolean value)
    {
        AllowNoArguments = value;
        return this;
    }

    
    /// <summary>
    /// Defaut argument setting.
    /// -> program /a /arg /arg:value
    /// </summary>
    public static ArgumentParserSettings Default = new("/", "/", ':');
    
    
    /// <summary>
    /// Unix style argument settings
    /// -> program -a --arg --arg=value
    /// </summary>
    public static ArgumentParserSettings UnixStyle = new("-", "--", '=');
}