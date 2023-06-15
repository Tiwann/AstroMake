namespace AstroMake;

public class WindowedApplication : Application
{
    public WindowedApplication(Workspace Workspace) : base(Workspace) => Type = OutputType.Windowed;
}