using System;
using System.Collections.Generic;

namespace AstroMake
{
    internal class MainClass
    {
        static void Main(List<String> Args)
        {
            Console.WriteLine(Args);
            Workspace ws = new Workspace("AstroExample");
            
        }
    }
}
