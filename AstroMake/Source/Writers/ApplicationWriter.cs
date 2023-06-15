using System;
using System.IO;
using System.Xml;

namespace AstroMake;

public class ApplicationWriter : IDisposable
{
    protected XmlTextWriter writer;

    public ApplicationWriter(Stream Output)
    {
        writer = XmlStatics.CreateWriter(Output);
    }

    public virtual void Write(Workspace Workspace, Application Application)
    {
        
    }

    public void Dispose()
    {
        writer?.Dispose();
    }
}