using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolutionZipper;

namespace SunZip
{
    /// <summary>
    /// SunZip is the conmmandline entrance to Zipping VS Solutions, CSharpProjects, SetupProjects and Items
    /// Lovingly called SunZip because sol in danish means sun.
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
            Console.WriteLine("SunZip usage:");
            Console.WriteLine("    {0} To Display help - same as no arguments", SolZipConstants.HelpArgument);
            Console.WriteLine("    {0} - this means that SolZipReadme.txt will not be added to Archive", SolZipConstants.ExcludeReadmeArgument);
            Console.WriteLine("    {0}:\"name\" The filename of the ZipFile.", SolZipConstants.ZipFileArgument);
            Console.WriteLine("    {0}:\"name\" The filename of the solution to Zip.", SolZipConstants.SolutionArgument);
            Console.WriteLine("    {0}:\"name\" The filename of the C# Project to Zip.", SolZipConstants.ProjectArgument);
            Console.WriteLine("    {0}:\"name\" The filename of the setup project to Zip.", SolZipConstants.SetupProjectArgument);
            Console.WriteLine("    {0}:\"name\" The file to Zip.", SolZipConstants.FileArgument);
            Console.WriteLine();
            Console.WriteLine("    Only one of {0}, {1}, {2} and {3} can be provided.", 
                SolZipConstants.SolutionArgument, SolZipConstants.ProjectArgument, SolZipConstants.SetupProjectArgument, 
                SolZipConstants.FileArgument);
            Console.WriteLine("    If no name is provided, first item in current directory is chosen.");
            Console.WriteLine();
            Console.WriteLine("    If no filename is provided for Zipfile a name is generated.");
            Console.WriteLine();
            Console.WriteLine("    Examples:");
            Console.WriteLine("      SunZip {0}:\"C:\\Eksempel.zip\" {1}:\"C:\\Eksempel\\Eksempel.sln\"", SolZipConstants.ZipFileArgument, SolZipConstants.SolutionArgument);
            Console.WriteLine("      SunZip {0}:\"C:\\Eksempel\\Eksempel\\Eksempel.csproj\"", SolZipConstants.ProjectArgument);
        }

        private static string GetZipFileName(Dictionary<string, string> args, string fileToZip)
        {
            if (!args.ContainsKey(SolZipConstants.ZipFileArgument) || string.IsNullOrEmpty(args[SolZipConstants.ZipFileArgument]))
                return SolZipHelper.GetZipFileName(fileToZip);

            return args[SolZipConstants.ZipFileArgument];
        }

        /// <summary>
        /// Number of correct arguments must equal one, since only one of Solution, Project, SetupProject, File is allowed
        /// This method counts the number of correct arguments. /zipfile and /excludereadme are not counted, since they are optional.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static int NumberOfCorrectArguments(Dictionary<string, string> args)
        {
            int count = 0;
            var correctArguments = new string[] { SolZipConstants.SolutionArgument, SolZipConstants.ProjectArgument, 
                SolZipConstants.SetupProjectArgument, SolZipConstants.FileArgument };
            foreach (var arg in args.Keys)
            {
                if (arg == SolZipConstants.ZipFileArgument || arg == SolZipConstants.ExcludeReadmeArgument)
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
            return GetArgument(args, SolZipConstants.SolutionArgument); ;
        }

        private static string GetProjectArgument(Dictionary<string, string> args)
        {
            return GetArgument(args, SolZipConstants.ProjectArgument); ;
        }

        private static string GetSetupProjectArgument(Dictionary<string, string> args)
        {
            return GetArgument(args, SolZipConstants.SetupProjectArgument); ;
        }

        private static string GetFileArgument(Dictionary<string, string> args)
        {
            return GetArgument(args, SolZipConstants.FileArgument); ;
        }

        private static bool GetExcludeReadmeArgument(Dictionary<string, string> args)
        {
            return args.ContainsKey(SolZipConstants.ExcludeReadmeArgument);
        }

        private static string GetArgument(Dictionary<string, string> args, string key)
        {
            string argValue = string.Empty;
            if (args.ContainsKey(key))
            {
                argValue = args[key];
            }
            return argValue;
        }

        private static void Zip(Dictionary<string, string> args)
        {
            string solutionFile = GetSolutionArgument(args);
            string projectFile = GetProjectArgument(args);
            string setupProjectFile = GetSetupProjectArgument(args);
            string itemFile = GetFileArgument(args); 

            string helpText = "Zips The {0}: {1} to {2}";

            if (!string.IsNullOrEmpty(solutionFile))
            {
                string zipFileName = GetZipFileName(args, solutionFile);
                Console.WriteLine(helpText, "solution", solutionFile, zipFileName);
                SolZipHelper.ZipSolution(GetZipFileName(args, solutionFile), solutionFile, GetExcludeReadmeArgument(args)); 
            }
            else if (!string.IsNullOrEmpty(projectFile))
            {
                string zipFileName = GetZipFileName(args, projectFile);
                Console.WriteLine(helpText, "project", projectFile, zipFileName);
                SolZipHelper.ZipProject(GetZipFileName(args, projectFile), projectFile, GetExcludeReadmeArgument(args));
            }
            else if (!string.IsNullOrEmpty(setupProjectFile))
            {
                string zipFileName = GetZipFileName(args, setupProjectFile);
                Console.WriteLine(helpText, "setup project", setupProjectFile, zipFileName);
                SolZipHelper.ZipSetupProject(GetZipFileName(args, setupProjectFile), setupProjectFile, GetExcludeReadmeArgument(args));
            }
            else if (!string.IsNullOrEmpty(itemFile))
            {
                string zipFileName = GetZipFileName(args, itemFile);
                Console.WriteLine(helpText, "file", itemFile, zipFileName);
                SolZipHelper.ZipItem(GetZipFileName(args, itemFile), itemFile, GetExcludeReadmeArgument(args));
            }
            Console.WriteLine("Done !");
        }
    }

    //Sample Test strings
    // /zipfile:"C:\Eks\Eks.zip" /solution:"C:\Projects\KbhKommune.Ask.backup\KbhKommune.Ask.sln"
    // /project:"C:\Projects\KbhKommune.Ask.backup\CrmDataGateway\CrmDataGateway.csproj"
    // /file:"C:\Projects\KbhKommune.Ask.backup\CrmDataGateway\CrmDataGateway.csproj"
}
