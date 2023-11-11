using System;

namespace AstroMake;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public abstract class BuildAttribute : Attribute
{
    
}
