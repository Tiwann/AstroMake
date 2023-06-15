namespace AstroMake;

public class SharedLibrary : Application
{
    public SharedLibrary(Workspace Workspace) : base(Workspace) => Type = OutputType.SharedLibrary;
}