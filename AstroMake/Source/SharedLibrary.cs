namespace AstroMake;

public class SharedLibrary : Project
{
    public SharedLibrary(Solution Solution) : base(Solution) => Type = OutputType.SharedLibrary;
}