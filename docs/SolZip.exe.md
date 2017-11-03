# SolZip.exe - The Command Line tool

SolZip.exe is a Command line tool for Zipping Visual Studio Solutions, Projects and Items in the Solution Explorer. 

## Installation

To Install SolZip.exe you just need to find and install [SolZip.msi](http://solzip.codeplex.com/releases/view/72939)

## Functionality

The install also installs a path variable so that SolZip.exe is available from alle command prompts on the machine.

Here is an example where the user wants to zip a solution:

![](SolZip.exe_Dosprompt.jpg)

## Parameters

SolZip.exe works with these parameters:

|| Parameter || Description || Sample ||
| /? | Displays help - same as no parameter | SolZip /? |
| /excludereadme | SolZipReadme.txt will not be added to the archive | SolZip /excludereadme |
| /file | The first file in current directory will be zipped as a single file | SolZip /file |
| /file:"name" | The file with "name" either in current directory or using full path will be zipped as a single file | SolZip /file:"C:\Temp\file1.txt" |
| /project | The first *.csproj file in current directory will be zipped as a project | SolZip /project | 
| /project:"name" | The file with "name" either in current directory or using full path will be zipped as a project | SolZip /project:"C:\Temp\files.csproj" |
| /solution | The first *.sln file in current directory will be zipped as a solution | SolZip /solution | 
| /solution:"name" | The file with "name" either in current directory or using full path will be zipped as a solution | SolZip /solution:"C:\Temp\files.sln" |
| /keepsccbindings | This means Source control bindings will not be removed from sln and csproj files | SolZip /keepsccbindings /solution:"C:\Temp\files.sln" |
| /zipfile:"name" | The name of the zip file to be made. A name is generated if this parameter is not provided. | SolZip /zipfile:"ClassLibrary.zip" | 

## Behind the scenes or how it is done

Solutions are zipped by iterating over the lines of the sln file and finding all relevant solution items, projects and setup projects, and zipping each of them, and finally zipping the sln file itself.

Projects are zipped by iterating over the relevant nodes in the csproj file using LinqToXml and finding all relevant items and zipping each of them, and finally zipping the csproj file itself.

Setup projects are zipped by iterating over the folder of the vdproj file and zipping all files in the folder including the vdproj file itself.

## Other options

If you want an easier way of getting Right-click menus for zipping you should go to the [SolZipMME](SolZipMME) tool of SolZip.

If you want a recipe for your software factory you should go to the [SolZipGuidance](SolZipGuidance) tool of SolZip.