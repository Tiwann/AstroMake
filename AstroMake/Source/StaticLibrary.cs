﻿namespace AstroMake;

public class StaticLibrary : Project
{
    public StaticLibrary(Solution Solution) : base(Solution) => Type = OutputType.StaticLibrary;
}