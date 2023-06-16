using System;
using System.IO;
using System.Text;
using System.Xml;

namespace AstroMake;

public class VcxprojWriter : ApplicationWriter
{

    public VcxprojWriter(Stream Output) : base(Output)
    {
    }
    
    private void WriteAttribute(String Name, String Value)
    {
        writer.WriteStartAttribute(Name);
        writer.WriteValue(Value);
        writer.WriteEndAttribute();
    }

    public override void Write(Solution Solution, Application Application)
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
                    foreach (Configuration Configuration in Solution.Configurations)
                    {
                        foreach (Architecture Architecture in Solution.Architectures)
                        {
                            foreach (String Platform in Solution.Platforms)
                            {
                                String ConfigName = Solution.Platforms.Count == 0 ? $"{Configuration.Name}|{Architecture}" : $"{Configuration.Name} {Platform}|{Architecture}";
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
