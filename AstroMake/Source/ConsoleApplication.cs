namespace AstroMake;

public class ConsoleApplication : Application
{
    public ConsoleApplication(Workspace Workspace) : base(Workspace) => Type = OutputType.Console;
}