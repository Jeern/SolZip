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
        /// Allowed Arguments
        ///     /? To Display help - same as no arguments
        ///     /ZipFile:"name" The filename of the ZipFile. 
        ///     /Solution:"name" The filename of the solution to Zip. 
        ///     /Project:"name" The filename of the C# Project to Zip. 
        ///     /SetupProject:"name" The filename of the setup project to Zip. 
        ///     /File:"name" The file to Zip.
        ///     Only one of /Solution, /Project and /SetupProject can be provided. If no name is provided, first item in current dir is chosen
        ///     If full filename is not provided the file will be assumed to be in current dir.
        ///     If no filename is provided for Zipfile a name is generated.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                Dictionary<string, string> argDic = SolZipHelper.ArgsToDictionary(args);
                if (argDic == null || argDic.Count() == 0 || argDic.ContainsKey("/?"))
                {
                    DisplayHelp();
                    return;
                }

                if (!argDic.ContainsKey("/solution") && !argDic.ContainsKey("/project") && !argDic.ContainsKey("/setupproject") && !argDic.ContainsKey("/file"))
                {
                    DisplayHelp();
                    return;
                }

                m_ZipFileName = GetZipFileName(argDic, @"C:\Eksempel.txt");
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

        private static string m_ZipFileName;
        private static string m_FileToZip;

        private static void DisplayHelp()
        {
            Console.WriteLine("SunZip usage:");
            Console.WriteLine("    /? To Display help - same as no arguments");
            Console.WriteLine("    /ZipFile:\"name\" The filename of the ZipFile.");
            Console.WriteLine("    /Solution:\"name\" The filename of the solution to Zip.");
            Console.WriteLine("    /Project:\"name\" The filename of the C# Project to Zip.");
            Console.WriteLine("    /SetupProject:\"name\" The filename of the setup project to Zip.");
            Console.WriteLine("    /File:\"name\" The file to Zip.");
            Console.WriteLine();
            Console.WriteLine("    Only one of /Solution, /Project and /SetupProject can be provided.");
            Console.WriteLine("    If no name is provided, first item in current directory is chosen.");
            Console.WriteLine();
            Console.WriteLine("    If no filename is provided for Zipfile a name is generated.");
        }

        private static string GetZipFileName(Dictionary<string, string> args, string fileToZip)
        {
            if (!args.ContainsKey("/zipfile") || string.IsNullOrEmpty(args["/zipfile"]))
                return SolZipHelper.SuggestFileName(fileToZip);

            return args["/zipfile"];
        }
    }
}
