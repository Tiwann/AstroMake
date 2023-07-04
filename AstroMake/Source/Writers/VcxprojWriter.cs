using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace AstroMake;

public class VcxprojWriter : IDisposable
{
    private readonly XmlTextWriter Writer;
    private readonly Project Project;

    public VcxprojWriter(Stream Output, Project Project)
    {
        Writer = XmlStatics.CreateWriter(Output);
        this.Project = Project;
    }

    private void WriteStartElement(string Name, params (string, string)[] Attributes)
    {
        Writer.WriteStartElement(Name);
        foreach ((string, string) Attribute in Attributes)
        {
            WriteAttribute(Attribute.Item1, Attribute.Item2);
        }
    }

    private void WriteAttribute(string Name, string Value)
    {
        Writer.WriteStartAttribute(Name);
        Writer.WriteValue(Value);
        Writer.WriteEndAttribute();
    }

    private void WriteProperty(string Name, string Value)
    {
        Writer.WriteStartElement(Name);
        Writer.WriteString(Value);
        Writer.WriteEndElement();
    }
    
    private void WriteProperty(string Name, bool Value)
    {
        Writer.WriteStartElement(Name);
        Writer.WriteString(Value.ToString());
        Writer.WriteEndElement();
    }
    
    private void WriteProperty(string Name, Guid Value)
    {
        Writer.WriteStartElement(Name);
        Writer.WriteString($"{{{Value}}}");
        Writer.WriteEndElement();
        
    }

    private string GetConfigType(OutputType Type)
    {
        switch (Type)
        {
            case OutputType.Console:
            case OutputType.Windowed:
                return "Application";
            case OutputType.SharedLibrary:
                return "SharedLibrary";
            case OutputType.StaticLibrary:
                return"StaticLibrary";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // TODO: Rewrite this
    private IEnumerable<string> GetFilesFromWildcard(string Wildcard)
    {
        if (Wildcard.Contains("*") && Wildcard.Contains("**"))
        {
            string SearchDirectory = Path.Combine(Project.Location,
                Wildcard.FindAndTrimEnd("**").TrimEnd('\\', '/'));
            if (!Directory.Exists(SearchDirectory)) return null;
            return Directory.EnumerateFiles(SearchDirectory, $"**{Path.GetExtension(Wildcard)}",
                SearchOption.AllDirectories);
        }

        if (Wildcard.Contains("*") && !Wildcard.Contains("**"))
        {
            string SearchDirectory = Path.Combine(Project.Location,
                Wildcard.FindAndTrimEnd("*").TrimEnd('\\', '/'));
            if (!Directory.Exists(SearchDirectory)) return null;
            return Directory.EnumerateFiles(SearchDirectory, $"*{Path.GetExtension(Wildcard)}",
                SearchOption.TopDirectoryOnly);
        }

        return null;
    }

    public void Write()
    {
        try
        {
            // Document begin
            Writer.WriteStartDocument();
            Writer.WriteComment($"Astro Make {Version.AstroVersion} generated vcxproj");
            Writer.WriteComment("Astro Make © Erwann Messoah 2023");
            Writer.WriteComment("\"https://github.com/Tiwann/AstroMake\"");

            WriteStartElement("Project", ("DefaultTargets", "Build"), ("xmlns", XmlStatics.XmlNamespace));
            WriteStartElement("ItemGroup", ("Label", "ProjectConfigurations"));
            foreach (Configuration Configuration in Project.Solution.Configurations)
            {
                if (Project.Solution.Platforms.Count > 0)
                {
                    foreach (string Platform in Project.Solution.Platforms)
                    {
                        string ConfigName = $"{Configuration.Name} {Platform}";
                        WriteStartElement("ProjectConfiguration", ("Include", $"{ConfigName}|{Project.Solution.Architecture}"));
                        WriteProperty("Configuration", ConfigName);
                        WriteProperty("Platform", $"{Project.Solution.Architecture}");
                        Writer.WriteEndElement();
                    }
                }
                else
                {
                    string ConfigName = $"{Configuration.Name}";
                    WriteStartElement("ProjectConfiguration", ("Include", $"{ConfigName}|{Project.Solution.Architecture}"));
                    WriteProperty("Configuration", ConfigName);
                    WriteProperty("Platform", $"{Project.Solution.Architecture}");
                    Writer.WriteEndElement();
                }
            }
            Writer.WriteEndElement();

            // Write globals
            WriteStartElement("PropertyGroup", ("Label", "Globals"));
            WriteProperty("ProjectGuid", $"{{{Project.Guid}}}");
            WriteProperty("ConfigurationType", GetConfigType(Project.Type));
            WriteProperty("CharacterSet", "Unicode");
            WriteProperty("PlatformToolset", "v143");
            WriteProperty("WindowsTargetPlatformVersion", "10.0");
            WriteProperty("RootNamespace", Project.Name);
            WriteProperty("IgnoreWarnCompileDuplicatedFilename", true);
            WriteProperty("Keyword", "Win32Proj");
            WriteProperty("OutDir", Project.BinariesDirectory.EndsWith("\\") ? Project.BinariesDirectory : $"{Project.BinariesDirectory}\\");
            WriteProperty("IntDir", Project.IntermediateDirectory.EndsWith("\\") ? Project.IntermediateDirectory : $"{Project.IntermediateDirectory}\\");
            Writer.WriteEndElement();
            
            Writer.WriteStartElement("ItemDefinitionGroup");
            Writer.WriteStartElement("ClCompile");
            Writer.WriteStartElement("LanguageStandard");
            string std;
            switch (Project.Dialect)
            {
                case Dialect.CPP20:
                    std = "stdcpp20";
                    break;
                case Dialect.CPP17:
                    std = "stdcpp17";
                    break;
                case Dialect.CPP14:
                    std = "stdcpp14";
                    break;
                case Dialect.CPP11:
                    std = "stdcpp11";
                    break;
                case Dialect.CPPLatest:
                    std = "stdcpplatest";
                    break;
                case Dialect.CSharp8:
                case Dialect.CSharp9:
                case Dialect.CSharp10:
                case Dialect.CSharp11:
                case Dialect.CSharpLatest:
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Writer.WriteString(std);
            Writer.WriteEndElement();
            
            // Enable modules if c++20
            if (Convert.ToBoolean((int)(Project.Flags & ProjectFlags.ModuleSupport)) && Project.Dialect is Dialect.CPP20)
            {
                WriteProperty("EnableModules", true);
            }
            
            // Enable multiprocessor compile
            if (Convert.ToBoolean((int)(Project.Flags & ProjectFlags.MultiProcessorCompile)))
            {
                WriteProperty("MultiProcessorCompilation", true);
            }
            
            Writer.WriteEndElement();
            Writer.WriteEndElement();

            Writer.WriteComment("Import Microsoft targets");
            Writer.WriteStartElement("ImportGroup");  
            WriteStartElement("Import", ("Project", @"$(VCTargetsPath)\Microsoft.Cpp.Default.props"));
            Writer.WriteEndElement();
            WriteStartElement("Import", ("Project", @"$(VCTargetsPath)\Microsoft.Cpp.props"));
            Writer.WriteEndElement();
            WriteStartElement("Import", ("Project", @"$(VCTargetsPath)\Microsoft.Cpp.targets"));
            Writer.WriteEndElement();
            Writer.WriteEndElement();
            
            if (!Project.Links.IsEmpty())
            {
                Writer.WriteComment("Project references");
                Writer.WriteStartElement("ItemGroup");
                List<Project> Projects = Project.Solution.Projects.Where(P => Project.Links.Contains(P.Name)).ToList();
                if (!Projects.IsEmpty())
                {
                    foreach (Project Proj in Projects)
                    {
                        WriteStartElement("ProjectReference", ("Include", @$"{Proj.TargetPath}"));
                        WriteProperty("ProjectReference", Proj.Guid);
                        Writer.WriteEndElement();
                    }
                }
                Writer.WriteEndElement();
            }
            

            List<string> Files = new List<string>();
            Files.AddRange(Project.Files.Where(F => File.Exists($"{Project.Location}/{F}")));
            Project.Files.ForEach(S =>
            {
                var Found = GetFilesFromWildcard(S);
                if (Found != null) Files.AddRange(Found);
            });

            List<string> AdditionalFiles = new List<string>();
            Files.AddRange(Project.AdditionalFiles.Where(F => File.Exists($"{Project.Location}/{F}")));
            Project.AdditionalFiles.ForEach(S =>
            {
                var Found = GetFilesFromWildcard(S);
                if (Found != null) AdditionalFiles.AddRange(Found);
            });

            IEnumerable<string> AvailableSourceExtensions = new[] { ".c", ".cpp", ".cxx", ".cc" };
            IEnumerable<string> AvailableHeaderExtensions = new[] { ".h", ".hpp", ".hxx", ".hh", ".inl", ".inc" };

            if (Files.Count != 0)
            {
                // Sources
                Writer.WriteComment("Including source files");
                Writer.WriteStartElement("ItemGroup");
                foreach (string File in Files)
                {
                    if (AvailableSourceExtensions.Any(E => File.EndsWith(E)))
                    {
                        Writer.WriteStartElement("ClCompile");
                        WriteAttribute("Include", File);
                        Writer.WriteEndElement();
                    }
                }

                Writer.WriteEndElement();

                // Headers
                Writer.WriteComment("Including header files");
                Writer.WriteStartElement("ItemGroup");
                foreach (string Source in Files)
                {
                    if (AvailableHeaderExtensions.Any(E => Source.EndsWith(E)) && File.Exists(Source))
                    {
                        Writer.WriteStartElement("ClInclude");
                        WriteAttribute("Include", Source);
                        Writer.WriteEndElement();
                    }
                }

                Writer.WriteEndElement();
            }

            if (AdditionalFiles.Count != 0)
            {
                // Additional files
                Writer.WriteComment("Including additional files");
                Writer.WriteStartElement("ItemGroup");
                foreach (string AddFile in AdditionalFiles)
                {
                    Writer.WriteStartElement("None");
                    WriteAttribute("Include", AddFile);
                    Writer.WriteEndElement();
                }

                Writer.WriteEndElement();
            }


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