namespace AstroMake;

public class StaticLibrary : Application
{
    public StaticLibrary(Workspace Workspace) : base(Workspace) => Type = OutputType.StaticLibrary;
}