using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

namespace SolZipBasis
{
    public class SolutionFileReader : IDisposable
    {
        private FileStream m_FileStream;
        private string m_SolutionFile;

        public SolutionFileReader(string solutionFile)
        {
            m_FileStream = new FileStream(solutionFile, FileMode.Open, FileAccess.Read);
            m_SolutionFile = solutionFile;
        }

        private List<string> GetAllLines()
        {
            return GetAllLinesWorker().ToList();
        }


        private IEnumerable<string> GetAllLinesWorker()
        {
            using (StreamReader reader = new StreamReader(m_FileStream))
            {
                while (!reader.EndOfStream)
                {
                    yield return reader.ReadLine();
                }                
            }
        }

        public List<string> GetRelevantItemsFullFileNames()
        {
            return GetRelevantItemsFullFileNamesWorker().Distinct().ToList();
        }

        private IEnumerable<string> GetRelevantItemsFullFileNamesWorker()
        {
            IEnumerable<string> shortNames = GetRelevantItems();
            foreach (string item in shortNames)
            {
                yield return Path.Combine(Path.GetDirectoryName(m_SolutionFile), item); 
            }
        }

        public IEnumerable<string> GetRelevantItems()
        {
            IEnumerable<string> allLines = TrimLines(GetAllLines());
            IEnumerable<string> solutionItems = GetSolutionItems(allLines);
            IEnumerable<string> csharpProjectItems = GetCsharpProjectItems(allLines);
            IEnumerable<string> setupProjectItems = GetSetupProjectItems(allLines);
            return solutionItems.Union(csharpProjectItems.Union(setupProjectItems)); 
        }

        private string GetItemFromTo(string line, string from, string to)
        {
            string toItem = GetItemTo(line, to);
            return GetItemFrom(toItem, from);
        }

        private string GetItemTo(string line, string to)
        {
            if (!line.Contains(to))
                return line;

            int index = line.IndexOf(to);
            return line.Substring(0, index + to.Length);
        }

        private string GetItemFrom(string line, string from)
        {
            if (!line.Contains(from))
                return line;

            int index = line.LastIndexOf(from);
            return line.Substring(index + from.Length);
        }

        private IEnumerable<string> GetCsharpProjectItems(IEnumerable<string> lines)
        {
            IEnumerable<string> projectLines = GetCsharpProjectLines(lines);
            foreach (string line in projectLines)
            {
                yield return GetItemFromTo(line, "\"", SolZipConstants.ProjectExtension);
            }
        }


        private IEnumerable<string> GetCsharpProjectLines(IEnumerable<string> lines)
        {
            return GetLineContaining(lines, SolZipConstants.ProjectExtension +  "\"");
        }

        private IEnumerable<string> GetSetupProjectItems(IEnumerable<string> lines)
        {
            IEnumerable<string> projectLines = GetSetupProjectLines(lines);
            foreach (string line in projectLines)
            {
                yield return GetItemFromTo(line, "\"", SolZipConstants.SetupProjectExtension);
            }
        }

        private IEnumerable<string> GetSetupProjectLines(IEnumerable<string> lines)
        {
            return GetLineContaining(lines, SolZipConstants.SetupProjectExtension + "\"");
        }

        private IEnumerable<string> GetLineContaining(IEnumerable<string> lines, string searchstring)
        {
            foreach (string line in lines)
            {
                if (line.Contains(searchstring))
                    yield return line;
            }
        }

        private IEnumerable<string> GetSolutionItems(IEnumerable<string> lines)
        {
            IEnumerable<string> solutionItemLines = GetSolutionItemLines(lines);
            return TrimLines(SplitList(solutionItemLines, '=', 1));
        }

        private IEnumerable<string> SplitList(IEnumerable<string> lines, char seperator, int index)
        {
            foreach (string line in lines)
            {
                string[] array = line.Split(seperator);
                yield return array[index]; 
            }
        }

        private IEnumerable<string> GetSolutionItemLines(IEnumerable<string> lines)
        {
            bool relevantLine = false;
            foreach (string line in lines)
            {
                if (relevantLine && line.StartsWith("EndProjectSection"))
                    relevantLine = false;

                if (relevantLine)
                    yield return line;

                if (!relevantLine && line.StartsWith("ProjectSection(SolutionItems)"))
                    relevantLine = true;
            }

        }

        private IEnumerable<string> TrimLines(IEnumerable<string> lines)
        {
            return
                from line in lines
                select line.Trim();
        }

        
        #region Best practice disposepattern.

        private bool m_AlreadyDisposed;

        /// <summary>
        /// Dispose the object (from IDisposable)
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~SolutionFileReader()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose that is able to distiguish between a Dispose initiated by the Garbage Collector (isDisposing = false) or code (isDisposing=true)
        /// </summary>
        /// <param name="isDisposing"></param>
        private void Dispose(bool isDisposing)
        {
            if (m_AlreadyDisposed)
                return;

            if (isDisposing)
            {
                FreeManaged();
            }
            FreeUnmanaged();
            m_AlreadyDisposed = true;
        }

        /// <summary>
        /// Frees all Managed Ressources. Can be overridden
        /// </summary>
        protected virtual void FreeManaged()
        {
            m_FileStream.Dispose();
        }

        /// <summary>
        /// Frees all Unmanaged Ressources. Can be overridden
        /// </summary>
        protected virtual void FreeUnmanaged()
        {
            // ... Frigør unmanaged resourcer her
        }

        #endregion


    }
}
