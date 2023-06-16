using System;
using System.IO;
using System.Xml;

namespace AstroMake;

public class ApplicationWriter : IDisposable
{
    protected XmlTextWriter writer;

    protected ApplicationWriter(Stream Output)
    {
        writer = XmlStatics.CreateWriter(Output);
    }

    public virtual void Write(Solution Solution, Application Application)
    {

    }


    public void Dispose()
    {
        writer.Dispose();
        writer.BaseStream?.Dispose();
    }
}