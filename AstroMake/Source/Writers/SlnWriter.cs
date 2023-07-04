﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AstroMake;

public struct SlnWriterSettings
{
    public char IndentationCharacter;
    public int IndentationSize;

    public SlnWriterSettings(char IndentationCharacter, int IndentationSize)
    {
        this.IndentationCharacter = IndentationCharacter;
        this.IndentationSize = IndentationSize;
    }

    public static readonly SlnWriterSettings Default = new('\t', 1);
}

public class SlnWriter : IDisposable
{
    private BuildTask Task;
    private StringBuilder Builder = new StringBuilder();
    private StreamWriter Writer;
    private readonly SlnWriterSettings Settings;

    private List<string> SectionNames = new List<string>();
    private int NumIndentation = 0;
    private Solution Solution;


    public SlnWriter(BuildTask Task, Stream Output, Solution Solution)
    {
        Writer = new StreamWriter(Output);
        Settings = SlnWriterSettings.Default;
        this.Solution = Solution;
        this.Task = Task;
    }

    public SlnWriter(BuildTask Task, Stream Output, Solution Solution, SlnWriterSettings Settings)
    {
        Writer = new StreamWriter(Output);
        this.Settings = Settings;
        this.Solution = Solution;
        this.Task = Task;
    }

    private void BeginSection(string SectionName, string Key, string Value)
    {
        Builder.Append($"{GetIndentation()}{SectionName}");
        NumIndentation += 1;
        Builder.AppendLine($"({Key}) = {Value}");
        SectionNames.Add(SectionName);
    }

    private void BeginSection(string SectionName)
    {
        Builder.AppendLine($"{GetIndentation()}{SectionName}");
        NumIndentation += 1;
        SectionNames.Add(SectionName);
    }

    private void EndSection()
    {
        NumIndentation -= 1;
        Builder.AppendLine($"{GetIndentation()}End{SectionNames.Last()}");
        SectionNames.RemoveAt(SectionNames.Count - 1);
    }

    private void WriteLine(string Text)
    {
        Builder.AppendLine($"{GetIndentation()}{Text}");
    }

    private void WriteProperty(string Key, string Value)
    {
        WriteLine($"{Key} = {Value}");
    }

    private string GetIndentation()
    {
        StringBuilder Indentation = new StringBuilder();
        for (int i = 0; i < NumIndentation; i++)
        {
            for (int j = 0; j < Settings.IndentationSize; j++)
            {
                Indentation.Append(Settings.IndentationCharacter.ToString());
            }
        }

        return Indentation.ToString();
    }


    public void Write()
    {
        // Header
        WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00");
        WriteLine("# Visual Studio 17");
        WriteLine($"# Astro Make {Version.AstroVersion} Generated Solution File");
        WriteLine("# (C) Erwann Messoah 2023");
        WriteLine("# https://github.com/AstroMake\n");
        
        foreach (Project Project in Solution.Projects)
        {
            BeginSection("Project", $"\"{{{Project.ProjectTypeGuid}}}\"", 
                $"\"{Project.Name}\", \"{Project.TargetPath}\", \"{{{Project.Guid}}}\"");
            EndSection();
        }

        WriteLine("");
        BeginSection("Global");
        BeginSection("GlobalSection", "SolutionConfigurationPlatforms", "preSolution");

        foreach (Configuration Configuration in Solution.Configurations)
        {
            if (Solution.Platforms.Count != 0)
            {
                foreach (string Platform in Solution.Platforms)
                {
                    WriteLine($"{Configuration.Name}|{Platform} = {Configuration.Name}|{Platform}");
                }
            }
            else
            {
                WriteLine($"{Configuration.Name}|{Solution.Architecture} = {Configuration.Name}|{Solution.Architecture}");
            }
        }

        EndSection();

        BeginSection("GlobalSection", "ProjectConfigurationPlatforms", "postSolution");
        foreach (Project Project in Solution.Projects)
        {
            foreach (Configuration Configuration in Solution.Configurations)
            {
                if (Solution.Platforms.Count != 0)
                {
                    foreach (string Platform in Solution.Platforms)
                    {
                        WriteLine($"{{{Project.Guid}}}.{Configuration.Name}|{Platform}.ActiveCfg = {Configuration.Name} {Platform}|{Solution.Architecture}");
                        WriteLine($"{{{Project.Guid}}}.{Configuration.Name}|{Platform}.Build.0 = {Configuration.Name} {Platform}|{Solution.Architecture}");
                    }
                }
                else
                {
                    WriteLine($"{{{Project.Guid}}}.{Configuration.Name}|{Solution.Architecture}.ActiveCfg = {Configuration.Name}|{Solution.Architecture}");
                    WriteLine($"{{{Project.Guid}}}.{Configuration.Name}|{Solution.Architecture}.Build.0 = {Configuration.Name}|{Solution.Architecture}");
                }
            }
        }

        EndSection();


        BeginSection("GlobalSection", "SolutionProperties", "preSolution");
        WriteProperty("HideSolutionNode", "FALSE");
        EndSection();
        EndSection();

        Writer.Write(Builder.ToString());
    }


    public void Dispose()
    {
        Writer.Dispose();
    }
}