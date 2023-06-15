using System;

namespace AstroMake;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class BuildAttribute : Attribute
{
    public BuildAttribute()
    {
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class WorkspaceAttribute : Attribute
{
    public WorkspaceAttribute()
    {
        
    }
}