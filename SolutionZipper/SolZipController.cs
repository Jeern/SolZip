using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Xml.Linq;
using System.Reflection;

namespace SolutionZipper
{
    public class SolZipController : IDisposable
    {
        private FileStream m_FileStream;
        private ZipOutputStream m_ZipStream;
        private ZipEntryFactory m_ZipEntryFactory;
        private string m_TopPath; //The Path below which we will Zip (and nowhere else)
        private bool m_ExcludeSZReadme;
        private bool m_SZReadmeAlreadyAdded;
        private bool m_SolutionOrProject = false; //True if this Zip is a solution or project

        private string BeginingOfPath
        {
            get { return Path.GetDirectoryName(m_TopPath); }
        }

        private string ZipEntryFileName(string fileName)
        {
            if(!fileName.StartsWith(BeginingOfPath))
                return CleanedUpFileName(fileName);

            return CleanedUpFileName(fileName.Substring(BeginingOfPath.Length)); 
        }

        private string CleanedUpFileName(string fileName)
        {
            if (fileName.Length > 1 && fileName.Substring(1, 1) == ":")
                return CleanedUpFileName(fileName.Substring(2));

            if (fileName.StartsWith(@"\"))
                return CleanedUpFileName(fileName.Substring(1));

            return fileName;
        }

        public SolZipController(string zipFileName, bool excludeSZReadme)
        {
            m_ExcludeSZReadme = excludeSZReadme;
            m_TopPath = string.Empty;
            m_FileStream = new FileStream(zipFileName, FileMode.CreateNew, FileAccess.Write);
            m_ZipStream = new ZipOutputStream(m_FileStream);
            m_ZipEntryFactory = new ZipEntryFactory(ZipEntryFactory.TimeSetting.CreateTimeUtc);
        }

        public void ZipSolution(string solutionFile)
        {
            if (!solutionFile.EndsWith(SolZipConstants.SolutionExtension))
                throw new ArgumentException(
                    string.Format("ZipSolution can only zip {0} files - not {1}",
                        SolZipConstants.SolutionExtension, solutionFile), "solutionFile");

            m_SolutionOrProject = true;
            SetTopPathFromFile(solutionFile);
            IEnumerable<string> files = null;
            using (var reader = new SolutionFileReader(solutionFile))
            {
                files = reader.GetRelevantItemsFullFileNames();
                foreach (string file in files)
                {
                    if (file.EndsWith(SolZipConstants.ProjectExtension))
                    {
                        ZipProject(file);
                    }
                    else if (file.EndsWith(SolZipConstants.SetupProjectExtension))
                    {
                        ZipSetupProject(file);
                    }
                    else
                    {
                        ZipFile(file);
                    }
                 }
            }
            ZipFile(solutionFile);
        }

        public void ZipSetupProject(string projectFile)
        {
            if (!projectFile.EndsWith(SolZipConstants.SetupProjectExtension))
                throw new ArgumentException(
                    string.Format("ZipSetupProject can only zip {0} files - not {1}",
                        SolZipConstants.SetupProjectExtension, projectFile), "projectFile");
            
            SetTopPathFromFile(projectFile);
            string setupDir = Path.GetDirectoryName(projectFile);
            string[] filesIndDir = Directory.GetFiles(setupDir, "*", SearchOption.TopDirectoryOnly);
            foreach (string file in filesIndDir)
            {
                ZipFile(file);
            }
        }

        public void ZipProject(string projectFile)
        {
            if (!projectFile.EndsWith(SolZipConstants.ProjectExtension))
                throw new ArgumentException(
                    string.Format("ZipProject can only zip {0} files - not {1}", 
                        SolZipConstants.ProjectExtension, projectFile), "projectFile");

            m_SolutionOrProject = true;
            SetTopPathFromFile(projectFile);

            XDocument projDoc = XDocument.Load(projectFile);
            var pn = XNamespace.Get("http://schemas.microsoft.com/developer/msbuild/2003");

            string projectFilePath = Path.GetDirectoryName(projectFile);

            var references =
                from element in projDoc.Elements(pn + "Project").Elements(pn + "ItemGroup").Elements(pn + "Reference").Elements(pn + "HintPath") 
                select element.Value;

            var itemElements =
                from element in projDoc.Elements(pn + "Project").Elements(pn + "ItemGroup").Elements()
                where element.Name.LocalName != "Reference" && element.Name.LocalName != "ProjectReference" 
                select element;

            var itemAttributes =
                from attribute in itemElements.Attributes("Include")
                select attribute.Value;

            foreach (string item in references.Union(itemAttributes))
            {
                ZipFile(Path.Combine(projectFilePath, item));
            }
            ZipFile(projectFile);
        }

        public void ZipFile(string fileName)
        {
            SetTopPathFromFile(fileName);
            AddReadme(m_TopPath, m_ExcludeSZReadme);

            if (!File.Exists(fileName))
            {
                //This might be an Empty folder
                ZipEmptyFolder(fileName);
                return;
            }

            //We will only do it if this file is under the Top folder, and it exists
            if (fileName.StartsWith(BeginingOfPath))
            {
                byte[] fileArray = ReadFile(fileName);
                ZipEntry entry = m_ZipEntryFactory.MakeFileEntry(ZipEntry.CleanName(ZipEntryFileName(fileName)));
                entry.Size = fileArray.Length;
                m_ZipStream.PutNextEntry(entry);
                m_ZipStream.Write(fileArray, 0, fileArray.Length);
            }
        }

        private void ZipEmptyFolder(string folderName)
        {
            if (Directory.Exists(folderName))
            {
                ZipEntry entry = m_ZipEntryFactory.MakeDirectoryEntry(ZipEntry.CleanName(ZipEntryFileName(folderName)));
                m_ZipStream.PutNextEntry(entry);
            }
        }

        public byte[] ReadFile(string fileName)
        {
            if (Path.GetExtension(fileName) == SolZipConstants.SolutionExtension)
                return ReadSolutionFile(fileName, true);

            if (Path.GetExtension(fileName) == SolZipConstants.ProjectExtension)
                return ReadProjectFile(fileName, true);

            return ReadFileWorker(fileName);
        }

        private byte[] ReadSolutionFile(string fileName, bool removeSourceControl)
        {
            if (!removeSourceControl)
                return ReadFileWorker(fileName);

            using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(file, true))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string line in GetSolutionLines(reader))
                    {
                        sb.AppendLine(line);
                    }
                    return String2Bytes(sb.ToString());
                }
            }
        }

        private IEnumerable<string> GetSolutionLines(StreamReader reader)
        {
            bool inSccSection = false;
            const string sccSectionStart = "GlobalSection(SourceCodeControl)";
            const string sccSectionEnd = "EndGlobalSection";
            var otherSccLines = new string[] 
                { 
                    "SccProjectName", 
                    "SccAuxPath", 
                    "SccLocalPath", 
                    "SccProvider" 
                };

            while(!reader.EndOfStream)
            {
                string solutionLine = reader.ReadLine();
                string trimmedReadLine = TrimmedSolutionLine(solutionLine);

                if(Startswith(trimmedReadLine, otherSccLines))
                    yield break;

                if(trimmedReadLine.StartsWith(sccSectionStart))
                {
                    inSccSection = true;
                    yield break;
                }

                if(trimmedReadLine.StartsWith(sccSectionEnd) && inSccSection)
                {
                    inSccSection = false;
                    yield break;
                }

                if(inSccSection)
                    yield break;

                yield return solutionLine;
            }
        }

        private string TrimmedSolutionLine(string line)
        {
            return line.TrimStart(' ', '\t');
        }

        private bool Startswith(string line, string[] startArray)
        {
            foreach (string start in startArray)
            {
                if (line.StartsWith(start))
                    return true;
            }
            return false;
        }

        private byte[] ReadProjectFile(string fileName, bool removeSourceControl)
        {
            if (!removeSourceControl)
                return ReadFileWorker(fileName);
            
            XElement doc = XElement.Load(fileName);
            XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";
            (from element in doc.Descendants(ns + "PropertyGroup").Elements()
             where element.Name.LocalName.StartsWith("Scc")
             select element).Remove();
            
            return String2Bytes(doc.ToString());
        }

        private byte[] String2Bytes(string doc)
        {
            UTF8Encoding encoding = new UTF8Encoding(true);
            return encoding.GetBytes(doc);
        }

        private byte[] ReadFileWorker(string fileName)
        {
            using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                byte[] bar = new byte[file.Length];
                file.Read(bar, 0, (int)file.Length);
                return bar;
            }
        }

        public void SetTopPathFromFile(string file)
        {
            if (string.IsNullOrEmpty(m_TopPath))
            {
                m_TopPath = Path.GetDirectoryName(file);
            }
        }

        /// <summary>
        /// This funny little thing adds a readme file called SZReadme.txt, which is an advertisement for SolutionZipper. 
        /// It is optional of course. I do not want to force anyone to have an extra readme file. For now it is on here as an experiment.
        /// </summary>
        /// <returns></returns>
        private void AddReadme(string topPath, bool excludeSZReadme)
        {
            if (excludeSZReadme || m_SZReadmeAlreadyAdded || !m_SolutionOrProject)
                return;
            m_SZReadmeAlreadyAdded = true;
            
            Stream readmeFile = Assembly.GetExecutingAssembly().GetManifestResourceStream("SolutionZipper.readme.txt");
            byte[] fileArray = new byte[readmeFile.Length];
            readmeFile.Read(fileArray, 0, (int)readmeFile.Length);
            //ZipEntry entry = m_ZipEntryFactory.MakeFileEntry(ZipEntry.CleanName(ZipEntryFileName(GetReadMeName(topPath, 0))));
            ZipEntry entry = m_ZipEntryFactory.MakeFileEntry(ZipEntry.CleanName("SolZipReadme.txt"));
            entry.Size = fileArray.Length;
            m_ZipStream.PutNextEntry(entry);
            m_ZipStream.Write(fileArray, 0, fileArray.Length);
        }

        //private string GetReadMeName(string topPath, int seed)
        //{
        //    string readmeFileName = string.Empty;
        //    if(seed == 0)
        //    {
        //        readmeFileName = Path.Combine(topPath, "SZReadme.txt");
        //    }
        //    else
        //    {
        //        readmeFileName = Path.Combine(topPath, string.Format("SZReadme{0}.txt", seed.ToString()));
        //    }
        //    if(File.Exists(readmeFileName))
        //        return GetReadMeName(topPath, seed + 1);

        //    return readmeFileName;
        //}

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
        ~SolZipController()
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
            m_ZipStream.Close();
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
