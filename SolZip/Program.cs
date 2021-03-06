﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolZipBasis;
using System.IO;

namespace SolZip
{
    /// <summary>
    /// SolZip is the conmmandline entrance to Zipping VS Solutions, CSharpProjects, SetupProjects and Items
    /// </summary>
    class Program
    {
        /// <summary>
        /// Zips Visual Studio Solutions, Projects and Single Files. The argument /? Displays help (as well as no arguments)
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                Dictionary<string, string> argDic = SolZipHelper.ArgsToDictionary(args);
                if (MustDisplayHelp(argDic))
                {
                    DisplayHelp();
                    return;
                }

                Zip(argDic);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
#if DEBUG
            finally
            {
                Console.WriteLine("Press Any Key");
                Console.ReadLine();
            }
#endif
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("SolZip usage:");
            Console.WriteLine("    {0} To Display help - same as no arguments", SolZipConstants.HelpArgument);
            Console.WriteLine("    {0} - this means that SolZipReadme.txt will not be added to Archive", SolZipConstants.ExcludeReadmeArgument);
            Console.WriteLine("    {0} - SourceControl bindings will not be removed in Zip file", SolZipConstants.KeepSourceControlArgument);
            Console.WriteLine("    {0}:\"name\" The filename of the ZipFile.", SolZipConstants.ZipFileArgument);
            Console.WriteLine("    {0}:\"name\" The filename of the solution to Zip.", SolZipConstants.SolutionArgument);
            Console.WriteLine("    {0}:\"name\" The filename of the C# Project to Zip.", SolZipConstants.ProjectArgument);
            Console.WriteLine("    {0}:\"name\" The file to Zip.", SolZipConstants.FileArgument);
            Console.WriteLine();
            Console.WriteLine("    Only one of {0}, {1} and {2} can be provided.", 
                SolZipConstants.SolutionArgument, SolZipConstants.ProjectArgument, SolZipConstants.FileArgument);
            Console.WriteLine("    If no name is provided, first item in current directory is chosen.");
            Console.WriteLine();
            Console.WriteLine("    If no filename is provided for Zipfile a name is generated.");
            Console.WriteLine();
            Console.WriteLine("    Examples:");
            Console.WriteLine("      SolZip {0}:\"C:\\Eksempel.zip\" {1}:\"C:\\Eksempel\\Eksempel.sln\"", SolZipConstants.ZipFileArgument, SolZipConstants.SolutionArgument);
            Console.WriteLine("      SolZip {0}:\"C:\\Eksempel\\Eksempel\\Eksempel.csproj\"", SolZipConstants.ProjectArgument);
        }

        private static string GetZipFileName(Dictionary<string, string> args, string fileToZip)
        {
            if (!args.ContainsKey(SolZipConstants.ZipFileArgument) || string.IsNullOrEmpty(args[SolZipConstants.ZipFileArgument]))
                return SolZipHelper.GetZipFileName(fileToZip);

            return args[SolZipConstants.ZipFileArgument];
        }

        /// <summary>
        /// Number of correct arguments must equal one, since only one of Solution, Project, SetupProject, File is allowed
        /// This method counts the number of correct arguments. /zipfile, /excludereadme  and /keepsccbindings are not counted, since they are optional.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static int NumberOfCorrectArguments(Dictionary<string, string> args)
        {
            int count = 0;
            var correctArguments = new string[] { SolZipConstants.SolutionArgument, SolZipConstants.ProjectArgument, 
                SolZipConstants.FileArgument };
            foreach (var arg in args.Keys)
            {
                if (arg == SolZipConstants.ZipFileArgument || arg == SolZipConstants.ExcludeReadmeArgument || arg == SolZipConstants.KeepSourceControlArgument)
                {
                    //OK, not counted since it is optional
                }
                else if (correctArguments.Contains(arg))
                {
                    count++;
                }
                else
                {
                    return -1;
                }
            }
            return count;
        }

        /// <summary>
        /// Number of correct arguments must equal one, since only one of Solution, Project, SetupProject, File is allowed
        /// This method counts the number of correct arguments. /zipfile is not counted, since it is optional.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static bool IsValidArguments(Dictionary<string, string> args)
        {
            if (args == null || args.Count() == 0)
                return false;

            return (NumberOfCorrectArguments(args) == 1);
        }

        private static bool MustDisplayHelp(Dictionary<string, string> args)
        {
            return !IsValidArguments(args) || args.ContainsKey(SolZipConstants.HelpArgument);
        }

        private static string GetSolutionArgument(Dictionary<string, string> args)
        {
            return GetArgument(args, SolZipConstants.SolutionArgument, "*" + SolZipConstants.SolutionExtension);
        }

        private static string GetProjectArgument(Dictionary<string, string> args)
        {
            return GetArgument(args, SolZipConstants.ProjectArgument, "*" + SolZipConstants.ProjectExtension); 
        }

        private static string GetFileArgument(Dictionary<string, string> args)
        {
            return GetArgument(args, SolZipConstants.FileArgument, "*.*");
        }

        private static bool GetExcludeReadmeArgument(Dictionary<string, string> args)
        {
            return args.ContainsKey(SolZipConstants.ExcludeReadmeArgument);
        }

        private static bool GetKeepSCCArgument(Dictionary<string, string> args)
        {
            return args.ContainsKey(SolZipConstants.KeepSourceControlArgument);
        }

        private static string GetArgument(Dictionary<string, string> args, string key, string pattern)
        {
            string fileName = string.Empty;
            if (args.ContainsKey(key))
            {
                fileName = args[key];
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = FindFirstFileWithPattern(pattern);
                }
                if (!string.IsNullOrEmpty(fileName))
                {
                    if (File.Exists(fileName))
                    {
                        var fileInfo = new FileInfo(fileName);
                        fileName = fileInfo.FullName;
                    }
                    else
                    {
                        Console.WriteLine("File {0} does not exist", fileName);
                        fileName = string.Empty;
                    }
                }
            }
            return fileName;
        }

        private static void Zip(Dictionary<string, string> args)
        {
            string solutionFile = GetSolutionArgument(args);
            string projectFile = GetProjectArgument(args);
            string itemFile = GetFileArgument(args); 

            string helpText = "Zips The {0}: {1} to {2}";

            if (!string.IsNullOrEmpty(solutionFile))
            {
                string zipFileName = GetZipFileName(args, solutionFile);
                Console.WriteLine(helpText, "solution", solutionFile, zipFileName);
                SolZipHelper.ZipSolution(GetZipFileName(args, solutionFile), solutionFile, GetExcludeReadmeArgument(args), !GetKeepSCCArgument(args)); 
            }
            else if (!string.IsNullOrEmpty(projectFile))
            {
                string zipFileName = GetZipFileName(args, projectFile);
                Console.WriteLine(helpText, "project", projectFile, zipFileName);
                SolZipHelper.ZipProject(GetZipFileName(args, projectFile), projectFile, GetExcludeReadmeArgument(args), !GetKeepSCCArgument(args));
            }
            else if (!string.IsNullOrEmpty(itemFile))
            {
                string zipFileName = GetZipFileName(args, itemFile);
                Console.WriteLine(helpText, "file", itemFile, zipFileName);
                SolZipHelper.ZipItem(GetZipFileName(args, itemFile), itemFile, GetExcludeReadmeArgument(args), !GetKeepSCCArgument(args));
            }
            Console.WriteLine("Done !");
        }

        private static string FindFirstFileWithPattern(string pattern)
        {
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), pattern, SearchOption.TopDirectoryOnly);
            if (files == null || files.Length == 0)
                return string.Empty;

            return files.First();
        }
    }

    //Sample Test strings
    // /zipfile:"C:\Eks\Eks.zip" /solution:"C:\Projects\KbhKommune.Ask.backup\KbhKommune.Ask.sln"
    // /project:"C:\Projects\KbhKommune.Ask.backup\CrmDataGateway\CrmDataGateway.csproj"
    // /file:"C:\Projects\KbhKommune.Ask.backup\CrmDataGateway\CrmDataGateway.csproj"
}
