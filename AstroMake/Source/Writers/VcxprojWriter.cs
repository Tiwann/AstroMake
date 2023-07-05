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
    
    private void WriteElement(string Name, (string, string) Attribute, params Action[] Content)
    {
        Writer.WriteStartElement(Name);
        WriteAttribute(Attribute.Item1, Attribute.Item2);
        foreach (Action Action in Content)
        {
            Action.Invoke();
        }
        Writer.WriteEndElement();
    }
    
    private void WriteElement(string Name, params Action[] Content)
    {
        Writer.WriteStartElement(Name);
        foreach (Action Action in Content)
        {
            Action.Invoke();
        }
        Writer.WriteEndElement();
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
    
    private void WriteProperty(string Name, CPPStandard Standard)
    {
        Writer.WriteStartElement(Name);
        Writer.WriteString(GetStandard(Standard));
        Writer.WriteEndElement();
    }
    
    private void WriteProperty(string Name, OutputType Type)
    {
        Writer.WriteStartElement(Name);
        Writer.WriteString(GetConfigType(Type));
        Writer.WriteEndElement();
    }

    private void WriteConfigurations(Solution Solutiuon)
    {
        WriteElement("ItemGroup", ("Label", "ProjectConfigurations"), delegate
        {
            foreach (Configuration Configuration in Solutiuon.Configurations)
            {
                if (Solutiuon.Platforms.Count > 0)
                {
                    foreach (string Platform in Solutiuon.Platforms)
                    {
                        string ConfigName = $"{Configuration.Name} {Platform}";
                        WriteStartElement("ProjectConfiguration", ("Include", $"{ConfigName}|{Solutiuon.Architecture}"));
                        WriteProperty("Configuration", ConfigName);
                        WriteProperty("Platform", $"{Solutiuon.Architecture}");
                        Writer.WriteEndElement();
                    }
                }
                else
                {
                    string ConfigName = $"{Configuration.Name}";
                    WriteStartElement("ProjectConfiguration", ("Include", $"{ConfigName}|{Solutiuon.Architecture}"));
                    WriteProperty("Configuration", ConfigName);
                    WriteProperty("Platform", $"{Solutiuon.Architecture}");
                    Writer.WriteEndElement();
                }
            }
        });
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

    private string GetStandard(CPPStandard Standard)
    {
        switch (Standard)
        {
            case CPPStandard.CPP20:
                return "stdcpp20";
            case CPPStandard.CPP17:
                return "stdcpp17";
            case CPPStandard.CPP14:
                return "stdcpp14";
            case CPPStandard.CPP11:
                return "stdcpp11";
            case CPPStandard.CPPLatest:
                return "stdcpplatest";
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
            Writer.WriteComment("Astro Make (C) Erwann Messoah 2023");
            Writer.WriteComment("\"https://github.com/Tiwann/AstroMake\"");

            WriteStartElement("Project", ("DefaultTargets", "Build"), ("xmlns", XmlStatics.XmlNamespace));
            WriteConfigurations(Project.Solution);
            
            // Write globals
            WriteElement("PropertyGroup", ("Label", "Globals"), delegate
            {
                WriteProperty("ProjectGuid", Project.Guid);
                WriteProperty("ConfigurationType", Project.Type);
                WriteProperty("CharacterSet", "Unicode");
                WriteProperty("PlatformToolset", "v143");
                WriteProperty("WindowsTargetPlatformVersion", "10.0");
                WriteProperty("RootNamespace", Project.Name);
                WriteProperty("IgnoreWarnCompileDuplicatedFilename", true);
                WriteProperty("Keyword", "Win32Proj");
                WriteProperty("OutDir", Project.BinariesDirectory.EndsWith("\\") ? Project.BinariesDirectory : $"{Project.BinariesDirectory}\\");
                WriteProperty("IntDir", Project.IntermediateDirectory.EndsWith("\\") ? Project.IntermediateDirectory : $"{Project.IntermediateDirectory}\\");
            });
            
            WriteElement("ItemDefinitionGroup", ("Label", "Globals"), delegate
            {
                WriteElement("ClCompile", delegate
                {
                    WriteProperty("LanguageStandard", Project.CppStandard);
            
                    // Enable modules if c++20 or above
                    if (Project.Flags.Contains(ProjectFlags.ModuleSupport) && Project.CppStandard >= CPPStandard.CPP20)
                    {
                        WriteProperty("EnableModules", true);
                    }
            
                    // Enable multiprocessor compile
                    if (Project.Flags.Contains(ProjectFlags.MultiProcessorCompile))
                    {
                        WriteProperty("MultiProcessorCompilation", true);
                    }
                });
            });
            

            // Import Microsoft targets and props
            Writer.WriteComment("Import Microsoft targets");
            WriteElement("ImportGroup", delegate
            {
                WriteElement("Import", ("Project", @"$(VCTargetsPath)\Microsoft.Cpp.Default.props"));
                WriteElement("Import", ("Project", @"$(VCTargetsPath)\Microsoft.Cpp.props"));
                WriteElement("Import", ("Project", @"$(VCTargetsPath)\Microsoft.Cpp.targets"));
            });
            
            if (!Project.Links.IsEmpty())
            {
                Writer.WriteComment("Project references");
                WriteElement("ItemGroup", delegate
                {
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
                });
            }
            
            // TODO: Better files including handling
            List<string> Files = new List<string>();
            Files.AddRange(Project.Files.Where(F => File.Exists(@$"{Project.Location}\{F}")));
            Project.Files.ForEach(S =>
            {
                var Found = GetFilesFromWildcard(S);
                if (Found != null) Files.AddRange(Found);
            });

            List<string> AdditionalFiles = new List<string>();
            Files.AddRange(Project.AdditionalFiles.Where(F => File.Exists(@$"{Project.Location}\{F}")));
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
                WriteElement("ItemGroup", ("Label", "Sources"), delegate
                {
                    foreach (string Source in Files)
                    {
                        if (AvailableSourceExtensions.Any(E => Source.EndsWith(E)))
                        {
                            Writer.WriteStartElement("ClCompile");
                            WriteAttribute("Include", Project.TargetDirectory.GetRelativePath(Path.Combine(Project.Location, Source)));
                            Writer.WriteEndElement();
                        }
                    }
                });

                // Headers
                Writer.WriteComment("Including header files");
                WriteElement("ItemGroup", ("Label", "Headers"), delegate
                {
                    foreach (string Header in Files)
                    {
                        if (AvailableHeaderExtensions.Any(E => Header.EndsWith(E)) && File.Exists(Header))
                        {
                            Writer.WriteStartElement("ClInclude");
                            WriteAttribute("Include", Path.GetFullPath(Header));
                            Writer.WriteEndElement();
                        }
                    }
                });
            }

            if (AdditionalFiles.Count != 0)
            {
                // Additional files
                Writer.WriteComment("Including additional files");
                WriteElement("ItemGroup", ("Label", "AdditionalFiles"), delegate
                {
                    foreach (string AddFile in AdditionalFiles)
                    {
                        Writer.WriteStartElement("AdditionalFiles");
                        WriteAttribute("Include", Path.GetFullPath(AddFile));
                        Writer.WriteEndElement();
                    }
                });
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