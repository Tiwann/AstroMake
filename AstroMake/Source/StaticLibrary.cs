namespace AstroMake;

public class StaticLibrary : Application
{
    public StaticLibrary(Solution Solution) : base(Solution) => Type = OutputType.StaticLibrary;
}