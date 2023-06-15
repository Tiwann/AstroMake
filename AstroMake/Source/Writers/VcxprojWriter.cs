using System;
using System.IO;
using System.Text;
using System.Xml;

namespace AstroMake;

public class VcxprojWriter : IDisposable
{
    private XmlTextWriter m_Writer;
    private Stream m_Output;
    
    public VcxprojWriter(Stream Output)
    {
        m_Output = Output;
        XmlWriterSettings Settings = new XmlWriterSettings
        {
            Async = false,
            CheckCharacters = true,
            CloseOutput = true,
            Encoding = Encoding.UTF8,
            IndentChars = "    ",
            NewLineHandling = NewLineHandling.Entitize,
            ConformanceLevel = ConformanceLevel.Document,
        };
        m_Writer = new XmlTextWriter(m_Output, Encoding.UTF8);
        m_Writer.Formatting = Formatting.Indented;
        m_Writer.Indentation = 4;
        m_Writer.Namespaces = true;
    }

    public void Dispose()
    {
        m_Writer?.Dispose();
        m_Output?.Dispose();
    }

    internal void WriteAttribute(String Name, String Value)
    {
        m_Writer.WriteStartAttribute(Name);
        m_Writer.WriteValue(Value);
        m_Writer.WriteEndAttribute();
    }

    public void WriteApplication(Workspace Workspace, Application Application)
    {
        try
        {
            m_Writer.WriteStartDocument();
            m_Writer.WriteComment($"Astro Make {Version.AstroVersion} generated vcxproj");
            m_Writer.WriteComment("Astro Make (c) Erwann Messoah 2023");
            m_Writer.WriteComment("https://github.com/Tiwann/AstroMake");

            m_Writer.WriteStartElement("Project");
            WriteAttribute("DefaultTargets", "Build");
            WriteAttribute("xmlns", XmlStatics.XmlNamespace);
                m_Writer.WriteStartElement("ItemGroup");
                WriteAttribute("Label", "ProjectConfigurations");
                    foreach (Configuration Configuration in Workspace.Configurations)
                    {
                        foreach (Architecture Architecture in Workspace.Architectures)
                        {
                            foreach (String Platform in Workspace.Platforms)
                            {
                                String ConfigName = Workspace.Platforms.Count == 0 ? $"{Configuration.Name}|{Architecture}" : $"{Platform} {Configuration.Name}|{Architecture}";
                                m_Writer.WriteStartElement("ProjectConfiguration");
                                WriteAttribute("Include", ConfigName);
                            
                                m_Writer.WriteStartElement("Configuration");
                                m_Writer.WriteString(ConfigName);
                                m_Writer.WriteEndElement();
                            
                                m_Writer.WriteStartElement("Platform");
                                m_Writer.WriteString($"{Architecture}");
                                m_Writer.WriteEndElement();
                            
                                m_Writer.WriteEndElement();
                            }
                            
                        }
                        
                    }
                m_Writer.WriteEndElement();
            m_Writer.WriteEndElement();
        }
        catch (Exception e)
        {
            Log.Error($"Error while writing Vcxproj: {e.Message}");
        }
    }
}
