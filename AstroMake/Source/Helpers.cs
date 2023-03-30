using System;

namespace AstroMake 
{
    public static class Helpers 
    {
        public static bool AllStringsEmpty(params String[] strings) 
        {
            foreach(String str in strings) 
            {
                if (!String.IsNullOrEmpty(str))
                    return false;
            }
            return true;
        }
    }
}
