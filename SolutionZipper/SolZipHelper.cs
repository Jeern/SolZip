using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SolutionZipper
{
    public static class SolZipHelper
    {
        public static string SuggestFileName(string fileToZip)
        {
            return SuggestFileName(Path.Combine(Path.GetPathRoot(fileToZip), Path.GetFileNameWithoutExtension(fileToZip)), 0);
        }

        private static string SuggestFileName(string fileNoExtension, int seed)
        {
            string suggestion = string.Empty;
            if(seed == 0)
            {
                suggestion = fileNoExtension + ".zip";  
            }
            else
            {
                suggestion = string.Format("{0}.{1}.zip", fileNoExtension, seed.ToString());
            }
            if (File.Exists(suggestion))
                return SuggestFileName(fileNoExtension, seed++);

            return suggestion;
        }

        public static Dictionary<string, string> ArgsToDictionary(string[] args)
        {
            return args.ToDictionary(arg => arg.Split(new char[] { ':' }, 2)[0].ToLower(), arg => (arg != null && arg.Contains(':')) ? arg.Split(new char[] { ':' }, 2)[1] : string.Empty); 
            
        }
    }
}
