using System;
using System.IO;
using System.Text;
using System.Xml;

namespace AstroMake;

public class VcxprojWriter : IDisposable
{
    private readonly XmlTextWriter Writer;
    private readonly Application Application;
    
    public VcxprojWriter(Stream Output, Application Application)
    {
        Writer = XmlStatics.CreateWriter(Output);
        this.Application = Application;
    }
    
    private void WriteAttribute(string Name, string Value)
    {
        Writer.WriteStartAttribute(Name);
        Writer.WriteValue(Value);
        Writer.WriteEndAttribute();
    }

    public void Write()
    {
        try
        {
            Writer.WriteStartDocument();
            Writer.WriteComment($"Astro Make {Version.AstroVersion} generated vcxproj");
            Writer.WriteComment("Astro Make © Erwann Messoah 2023");
            Writer.WriteComment("https://github.com/Tiwann/AstroMake");

            Writer.WriteStartElement("Project");
            WriteAttribute("DefaultTargets", "Build");
            WriteAttribute("xmlns", XmlStatics.XmlNamespace);
                Writer.WriteStartElement("ItemGroup");
                WriteAttribute("Label", "ProjectConfigurations");
                    foreach (Configuration Configuration in Application.Solution.Configurations)
                    {
                        foreach (Architecture Architecture in Application.Solution.Architectures)
                        {
                            if (Application.Solution.Platforms.Count > 0)
                            {
                                foreach (string Platform in Application.Solution.Platforms)
                                {
                                    string ConfigName = $"{Configuration.Name} {Platform}|{Architecture}";
                                    Writer.WriteStartElement("ProjectConfiguration");
                                    WriteAttribute("Include", ConfigName);
                            
                                    Writer.WriteStartElement("Configuration");
                                    Writer.WriteString(ConfigName);
                                    Writer.WriteEndElement();
                            
                                    Writer.WriteStartElement("Platform");
                                    Writer.WriteString($"{Architecture}");
                                    Writer.WriteEndElement();
                            
                                    Writer.WriteEndElement();
                                }
                            }
                            else
                            {
                                string ConfigName = $"{Configuration.Name}|{Architecture}";
                                Writer.WriteStartElement("ProjectConfiguration");
                                WriteAttribute("Include", ConfigName);
                            
                                Writer.WriteStartElement("Configuration");
                                Writer.WriteString(ConfigName);
                                Writer.WriteEndElement();
                            
                                Writer.WriteStartElement("Platform");
                                Writer.WriteString($"{Architecture}");
                                Writer.WriteEndElement();
                            
                                Writer.WriteEndElement();
                            }
                        }
                        
                    }
                Writer.WriteEndElement();
            Writer.WriteEndElement();
        }
        catch (Exception Exception)
        {
            Log.Error($"Error while writing Vcxproj: {Exception.Message}");
        }
    }

    public void Dispose()
    {
        Writer?.Dispose();
    }
}
