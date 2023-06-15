using System;
using System.IO;
using System.Xml;

namespace AstroMake;

public class CsprojWriter : IDisposable
{
    private XmlTextWriter writer;
    private Stream stream;
    
    
    

    public void Dispose()
    {
        writer.Dispose();
        stream.Dispose();
    }
}