# <img src="/Resources/AstroMakeLogo.png" width="32" height="32">  Astro Make

## Introduction
Astro Make is a simple and fast build tool that generates
visual studio solutions and projects. <br>
It works like premake and cmake but it uses _C# scripts_ to describe a solution for generation. 
The idea is to keep the scripts writing and reading as _simple and fast_ as possible. <br>
Using C# helps for **syntax highlighting** and **code completion**. 
For now Astro Make is only available for Windows. But we'll may be porting it for other platforms.


## How it works
When running Astro Make program, it recursively look for `.Astro.cs` files that will be _compiled_ 
into an assembly in runtime.
The compiler gives error messages so the users know if their code have mistakes.

## How to use





