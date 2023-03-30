using System;


namespace AstroMake
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class Build : Attribute
    {
        public Build()
        {
            
        }
    }
}