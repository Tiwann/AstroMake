﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AstroMakeInstaller;

internal static class Program
{

    public static readonly Uri AstroMakeLatest = new Uri("https://github.com/tiwann/astromake/releases/latest/download/AstroMake-Latest-Windows-x64.exe");
    
    public static void Main()
    {
        Console.WriteLine("Welcome to Astro make installer!");
        Console.WriteLine("Downloading latest version of Astro Make...");
        
        Task<byte[]> File = new HttpClient().GetByteArrayAsync(AstroMakeLatest);
        
        using FileStream FStream = new FileStream("AstroMake.exe", FileMode.Create, FileAccess.Write);
        var WriteTask = FStream.WriteAsync(File.Result);
    }
}