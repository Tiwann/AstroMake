using System;
using System.IO;
using System.Xml;

namespace AstroMake;

public class CsprojWriter : IDisposable
{
    private XmlTextWriter writer;

    public CsprojWriter(Stream Output)
    {
        writer = XmlStatics.CreateWriter(Output);
    }

    
    public void Dispose()
    {
        writer?.Dispose();
    }
}