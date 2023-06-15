using System;
using System.IO;
using System.Text;
using System.Xml;

namespace AstroMake;

public class VcxprojWriter : IDisposable
{
    private readonly XmlTextWriter writer;

    public VcxprojWriter(Stream Output)
    {
        writer = XmlStatics.CreateWriter(Output);
    }

    public void Dispose()
    {
        writer?.Dispose();
    }

    private void WriteAttribute(String Name, String Value)
    {
        writer.WriteStartAttribute(Name);
        writer.WriteValue(Value);
        writer.WriteEndAttribute();
    }

    public void WriteApplication(Workspace Workspace, Application Application)
    {
        try
        {
            writer.WriteStartDocument();
            writer.WriteComment($"Astro Make {Version.AstroVersion} generated vcxproj");
            writer.WriteComment("Astro Make (c) Erwann Messoah 2023");
            writer.WriteComment("https://github.com/Tiwann/AstroMake");

            writer.WriteStartElement("Project");
            WriteAttribute("DefaultTargets", "Build");
            WriteAttribute("xmlns", XmlStatics.XmlNamespace);
                writer.WriteStartElement("ItemGroup");
                WriteAttribute("Label", "ProjectConfigurations");
                    foreach (Configuration Configuration in Workspace.Configurations)
                    {
                        foreach (Architecture Architecture in Workspace.Architectures)
                        {
                            foreach (String Platform in Workspace.Platforms)
                            {
                                String ConfigName = Workspace.Platforms.Count == 0 ? $"{Configuration.Name}|{Architecture}" : $"{Platform} {Configuration.Name}|{Architecture}";
                                writer.WriteStartElement("ProjectConfiguration");
                                WriteAttribute("Include", ConfigName);
                            
                                writer.WriteStartElement("Configuration");
                                writer.WriteString(ConfigName);
                                writer.WriteEndElement();
                            
                                writer.WriteStartElement("Platform");
                                writer.WriteString($"{Architecture}");
                                writer.WriteEndElement();
                            
                                writer.WriteEndElement();
                            }
                            
                        }
                        
                    }
                writer.WriteEndElement();
            writer.WriteEndElement();
        }
        catch (Exception e)
        {
            Log.Error($"Error while writing Vcxproj: {e.Message}");
        }
    }
}
