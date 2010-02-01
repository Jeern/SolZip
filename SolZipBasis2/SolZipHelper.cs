using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SolZipBasis2
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

        public static void ZipItem(string zipFileName, string fileToZip, bool excludeSZReadme, bool removeSourceControl)
        {
            using (var controller = new SolZipController(zipFileName, excludeSZReadme, removeSourceControl))
            {
                controller.ZipFile(fileToZip);
            }
        }

        public static void ZipProject(string zipFileName, string fileToZip, bool excludeSZReadme, bool removeSourceControl)
        {
            using (var controller = new SolZipController(zipFileName, excludeSZReadme, removeSourceControl))
            {
                controller.ZipProject(fileToZip);
            }
        }

        public static void ZipSolution(string zipFileName, string fileToZip, bool excludeSZReadme, bool removeSourceControl)
        {
            using (var controller = new SolZipController(zipFileName, excludeSZReadme, removeSourceControl))
            {
                controller.ZipSolution(fileToZip);
            }
        }

        public static void Zip(string zipFileName, string fileToZip, bool excludeSZReadme, bool removeSourceControl)
        {
            using (var controller = new SolZipController(zipFileName, excludeSZReadme, removeSourceControl))
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

        public static void Zip(string fileToZip, bool excludeSZReadme, bool removeSourceControl)
        {
            Zip(GetZipFileName(fileToZip), fileToZip, excludeSZReadme, removeSourceControl);
        }

        public static void Zip(string zipFileName, string fileToZip, bool showFile, 
            bool copyToClipboard, bool excludeReadme, bool removeSourceControl)
        {
            if (copyToClipboard)
            {
                CopyFileNameToClipboard(zipFileName);
            }
            Zip(zipFileName, fileToZip, excludeReadme, removeSourceControl);
            if (showFile)
            {
                Process.Start(zipFileName);
            } 
        }

        public static void CopyFileNameToClipboard(string zipFileName)
        {
            try
            {
                Clipboard.SetText(zipFileName);
            }
            catch (ExternalException)
            {
                //Apparently this exception occurs on Windows Vista. Seems to have no effect though.
                //So I just "eat" it. Not nice but... Apparently it should be possible to control this
                //with the UIPermission class, for now, no success however.
            }
        }

        public static string CleanedUpFileName(string fileName)
        {
            if (fileName.Length > 1 && fileName.Substring(1, 1) == ":")
                return CleanedUpFileName(fileName.Substring(2));

            if (fileName.StartsWith(@"\"))
                return CleanedUpFileName(fileName.Substring(1));

            if (fileName.StartsWith(@".\"))
                return CleanedUpFileName(fileName.Substring(2));

            fileName = RemoveSingleDotFromFilePath(fileName);
            fileName = RemoveDoubleDotFromFilePath(fileName);

            return fileName;
        }

        /// <summary>
        /// This method is here because WinZip has as problem wih paths containing .. and probably . too
        /// This method removes all instances \.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string RemoveSingleDotFromFilePath(string fileName)
        {
            int pos = fileName.IndexOf(@"\.\");

            if (pos < 0)
                return fileName;

            if (fileName.Length == 3)
                return fileName;

            if (pos == 0)
                return RemoveSingleDotFromFilePath(fileName.Substring(3));

            return RemoveSingleDotFromFilePath(fileName.Substring(0, pos) + fileName.Substring(pos + 2));

        }

        /// <summary>
        /// This method is here because WinZip has as problem wih paths containing .. and probably . too
        /// This method removes all instances \..
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string RemoveDoubleDotFromFilePath(string fileName)
        {
            int pos = fileName.IndexOf(@"\..");

            if (pos < 0)
                return fileName;

            if (fileName.Length == 3)
                return fileName;

            if (pos == 0)
                return RemoveDoubleDotFromFilePath(fileName.Substring(1));

            int posParent = fileName.Substring(0, pos).LastIndexOf(@"\");

            if (posParent <= 0)
                return RemoveDoubleDotFromFilePath(fileName.Substring(pos + 3));

            return RemoveDoubleDotFromFilePath(fileName.Substring(0, posParent) + fileName.Substring(pos + 3));

        }

        /// <summary>
        /// Returns the first linenumber of the ContentLines containing searchstring. Lines start at 0.
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="startPosition"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetLineNumbersContaining(List<string> lines, string searchString, int startPosition)
        {
            int index = -1;
            foreach(string line in lines)
            {
                index++;
                if (line.Contains(searchString) && index >= startPosition)
                {
                    yield return index;
                }
            }
        }

        /// <summary>
        /// Iterates from the first to the last integer and returns a list of them
        /// </summary>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetListOfIntegers(int first, int last)
        {
            if (first > last)
                throw new InvalidOperationException(string.Format("First is after last. First: {1} Last: {2}", first.ToString(), last.ToString()));

            for (int i = first; i <= last; i++)
            {
                yield return i;
            }
        }

    }
}
