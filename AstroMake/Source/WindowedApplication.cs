namespace AstroMake;

public class WindowedApplication : Application
{
    public WindowedApplication(Solution Solution) : base(Solution) => Type = OutputType.Windowed;
}