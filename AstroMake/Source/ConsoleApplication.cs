namespace AstroMake;

public class ConsoleApplication : Project
{
    public ConsoleApplication(Solution Solution) : base(Solution) => Type = OutputType.Console;
}