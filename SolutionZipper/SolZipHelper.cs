using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace SolutionZipper
{
    public static class SolZipHelper
    {
        private static string SuggestFileName(string fileToZip)
        {
            return SuggestFileName(Path.Combine(Path.GetDirectoryName(fileToZip), Path.GetFileNameWithoutExtension(fileToZip)),
                GetZipFileSuffix(fileToZip), 0);
        }

        private static string SuggestFileName(string fileNoExtension, string zipFileSuffix, int seed)
        {
            string suggestion = string.Empty;
            if(seed == 0)
            {
                suggestion = fileNoExtension + zipFileSuffix + ".zip";  
            }
            else
            {
                suggestion = string.Format("{0}{1}{2}.zip", fileNoExtension, zipFileSuffix, seed.ToString());
            }
            if (File.Exists(suggestion))
                return SuggestFileName(fileNoExtension, zipFileSuffix, seed+1);

            return suggestion;
        }

        private static string GetZipFileSuffix(string fileToZip)
        {
            switch (Path.GetExtension(fileToZip))
            {
                case SolZipConstants.SolutionExtension:
                    return SolZipConstants.SolutionSuffix; 
                case SolZipConstants.ProjectExtension:
                    return SolZipConstants.ProjectSuffix; 
                default:
                    return SolZipConstants.ItemSuffix;
            }
        }

        public static Dictionary<string, string> ArgsToDictionary(string[] args)
        {
            return args.ToDictionary(arg => arg.Split(new char[] { ':' }, 2)[0].ToLower(), arg => (arg != null && arg.Contains(':')) ? arg.Split(new char[] { ':' }, 2)[1] : string.Empty); 
            
        }

        public static string GetZipFileName(string fileToZip)
        {
            return SolZipHelper.SuggestFileName(fileToZip);
        }

        public static void ZipItem(string zipFileName, string fileToZip, bool excludeSZReadme)
        {
            using (var controller = new SolZipController(zipFileName, excludeSZReadme))
            {
                controller.ZipFile(fileToZip);
            }
        }

        public static void ZipProject(string zipFileName, string fileToZip, bool excludeSZReadme)
        {
            using (var controller = new SolZipController(zipFileName, excludeSZReadme))
            {
                controller.ZipProject(fileToZip);
            }
        }

        public static void ZipSolution(string zipFileName, string fileToZip, bool excludeSZReadme)
        {
            using (var controller = new SolZipController(zipFileName, excludeSZReadme))
            {
                controller.ZipSolution(fileToZip);
            }
        }

        public static void Zip(string zipFileName, string fileToZip, bool excludeSZReadme)
        {
            using (var controller = new SolZipController(zipFileName, excludeSZReadme))
            {
                switch (GetZipFileSuffix(fileToZip))
                {
                    case SolZipConstants.SolutionSuffix:
                        controller.ZipSolution(fileToZip);
                        break;
                    case SolZipConstants.ProjectSuffix:
                        controller.ZipProject(fileToZip);
                        break;
                    case SolZipConstants.ItemSuffix:
                        controller.ZipFile(fileToZip);
                        break;
                    default:
                        throw new ArgumentException("Suffix of file to zip not reqcognized", "fileToZip");
                }
            }
        }

        public static void Zip(string fileToZip, bool excludeSZReadme)
        {
            Zip(GetZipFileName(fileToZip), fileToZip, excludeSZReadme);
        }

        public static void Zip(string zipFileName, string fileToZip, bool showFile, bool copyToClipboard, bool excludeReadme)
        {
            if (copyToClipboard)
            {
                CopyFileNameToClipboard(zipFileName);
            }
            Zip(zipFileName, fileToZip, excludeReadme);
            if (showFile)
            {
                Process.Start(zipFileName);
            } 
        }

        private static void CopyFileNameToClipboard(string zipFileName)
        {
            Clipboard.SetText(zipFileName);
        }
    }
}
