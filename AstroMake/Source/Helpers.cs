using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AstroMake;

public static class Helpers 
{
    public static List<String> GetAllFilesWithExtension(String Directory, String Extension, bool Recursive)
    {
        List<String> Result = new();

        void CollectFileNames(String Dir)
        {
            foreach (String File in global::System.IO.Directory.GetFiles(Dir))
            {
                if(File.EndsWith(Extension))
                    Result.Add(File);
            }

            String[] SubDirectories = global::System.IO.Directory.GetDirectories(Dir);

            if (Recursive)
            {
                foreach (String SubDir in SubDirectories)
                    CollectFileNames(SubDir);
            }
        }
        
        CollectFileNames(Directory);
        return Result;
    }


    public static bool HasAttribute(this Type Type, Type AttributeType)
    {
        return Type.GetCustomAttribute(AttributeType) != null;
    }
    
}