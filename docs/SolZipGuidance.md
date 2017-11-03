# SolZipGuidance

SolZipGuidance uses Guidance Automation Extensions (GAX) from Microsoft to enable Right click menus on Visual Studio Solutions, Projects and Items in the Solution Explorer. You have to enable the package for each solution you need to Zip, and therefore the more interesting approach is to include the recipe in your own Software Factory Packages.

# Installation

To Install SolZipGuidance you need first need to install Guidance Automation Extensions. 

# Find and install GAX 2010. Easiest way is to install it from the Extension Manager in Visual Studio 2010. If you want to develop your own factory you will also want to install GAT 2010 in the same way.
# Find and install [SolZipGuidance.vsix](http://solzip.codeplex.com/releases/view/72939) - to get the "Compress with Zip..:" menu on C# Solutions, Projects and Items in VS2010. 

## Functionality

If you want use the SolZipGuidance as is, you first need to enable the package when you have opened a Visual Studio Solution.

Choose "Guidance Package Manager":

![](SolZipGuidance_SolZipGAX2.jpg)

Then "Enable/Disable Packages...":

![](SolZipGuidance_SolZipGAX3.jpg)

Now you can enable SolZipGuidance:

![](SolZipGuidance_SolZipGAX4.jpg)

Which gives you almost the same menu as SolZipMME:

![](SolZipGuidance_SolZipGAX1.jpg)

If you click "Compress with Zip..." you get this window:

![](SolZipGuidance_SolZipGAX5.jpg)

Where you can Zip the chosen object when "Finish" is clicked.

The checkboxes have the following meaning. "Show resulting file" means that the Zip file will be opened after the Zipping is done. "Copy to clipboard" means the full path and filename of the file will be copied to the clipboard. And "Exclude SolZipReadme.txt" means that a very small text file promoting this site will not be included in the Zip archive.

## The real power of SolZipGuidance

As you can see this process is more cumbersome than if you use [SolZipMME](SolZipMME) the strength of SolZipGuidance is that you can incorporate the recipe in your own Software Factories. You just need the SolZipRecipe.xml +  source code + some basic knowledge about Guidance Automation :O)

## Behind the scenes or how it is done

Solutions are zipped by iterating over the lines of the sln file and finding all relevant solution items, projects and setup projects, and zipping each of them, and finally zipping the sln file itself.

Projects are zipped by iterating over the relevant nodes in the csproj file using LinqToXml and finding all relevant items and zipping each of them, and finally zipping the csproj file itself.

Setup projects are zipped by iterating over the folder of the vdproj file and zipping all files in the folder including the vdproj file itself.

## Other options

If you want a standalone command line tool you should go to the [SolZip.exe](SolZip.exe) tool of SolZip.

If you an easier way of getting Right-click menus for zipping you should go to the [SolZipMME](SolZipMME) tool of SolZip.