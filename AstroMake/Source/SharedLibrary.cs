namespace AstroMake;

public class SharedLibrary : Application
{
    public SharedLibrary(Solution Solution) : base(Solution) => Type = OutputType.SharedLibrary;
}