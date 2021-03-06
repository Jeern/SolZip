﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Xml.Linq;
using System.Reflection;

namespace SolZipBasis
{
    public class SolZipController : IDisposable
    {
        private FileStream m_FileStream;
        private ZipOutputStream m_ZipStream;
        private ZipEntryFactory m_ZipEntryFactory;
        private string m_TopPath; //The Path below which we will Zip (and nowhere else)
        private bool m_ExcludeSZReadme;
        private bool m_RemoveSourceControl;
        private bool m_SZReadmeAlreadyAdded;
        private bool m_SolutionOrProject = false; //True if this Zip is a solution or project
        private Dictionary<string, string> m_ZippedFiles = new Dictionary<string, string>(); //contains the names of the zipped files

        private string BeginingOfPath
        {
            get { return Path.GetDirectoryName(m_TopPath); }
        }

        private string ZipEntryFileName(string fileName)
        {
            if(!fileName.StartsWith(BeginingOfPath))
                return SolZipHelper.CleanedUpFileName(fileName);

            return SolZipHelper.CleanedUpFileName(fileName.Substring(BeginingOfPath.Length)); 
        }

        public SolZipController(string zipFileName, bool excludeSZReadme, bool removeSourceControl)
        {
            m_ExcludeSZReadme = excludeSZReadme;
            m_RemoveSourceControl = removeSourceControl;
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
                    if (file.EndsWith(SolZipConstants.ProjectExtension) || file.EndsWith(SolZipConstants.ContentProjectExtension))
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
            if (!projectFile.EndsWith(SolZipConstants.ProjectExtension) && !projectFile.EndsWith(SolZipConstants.ContentProjectExtension))
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
                ZipFile(ReplacePlaceholderVars(projectFilePath, item));
            }
            ZipFile(projectFile);
        }

        /// <summary>
        /// Only recognizes $(SolutionDir) and $(ProjectDir) for now.
        /// </summary>
        /// <param name="projectFilePath"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private string ReplacePlaceholderVars(string projectFilePath, string item)
        {
            const string SolutionDir = "$(SolutionDir)";
            const string ProjectDir = "$(ProjectDir)";

            if (item.StartsWith(SolutionDir))
            {
                return ReplacePlaceholderVar(SolutionDir, m_TopPath, item);
            }
            else if (item.StartsWith(ProjectDir))
            {
                return ReplacePlaceholderVar(ProjectDir, projectFilePath, item);
            }
            else
            {
                return Path.Combine(projectFilePath, item);
            }
        }

        private string ReplacePlaceholderVar(string var, string replacement, string item)
        {
            return Path.Combine(replacement, item.Substring(var.Length));
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
                string zipEntryFileName = ZipEntry.CleanName(ZipEntryFileName(fileName));
                if (!m_ZippedFiles.ContainsKey(zipEntryFileName))
                {
                    ZipEntry entry = m_ZipEntryFactory.MakeFileEntry(zipEntryFileName);
                    entry.Size = fileArray.Length;
                    m_ZipStream.PutNextEntry(entry);
                    m_ZipStream.Write(fileArray, 0, fileArray.Length);
                    m_ZippedFiles.Add(zipEntryFileName, zipEntryFileName);
                }
            }
        }

        private void ZipEmptyFolder(string folderName)
        {
            if (Directory.Exists(folderName))
            {
                string zipEntryFolderName = ZipEntry.CleanName(ZipEntryFileName(folderName));
                if (!m_ZippedFiles.ContainsKey(zipEntryFolderName))
                {
                    ZipEntry entry = m_ZipEntryFactory.MakeDirectoryEntry(zipEntryFolderName);
                    m_ZipStream.PutNextEntry(entry);
                    m_ZippedFiles.Add(zipEntryFolderName, zipEntryFolderName);
                }
            }
        }

        private byte[] ReadFile(string fileName)
        {
            if (Path.GetExtension(fileName) == SolZipConstants.SolutionExtension)
                return ReadSolutionFile(fileName);

            if (Path.GetExtension(fileName) == SolZipConstants.ProjectExtension || Path.GetExtension(fileName) == SolZipConstants.ContentProjectExtension)
                return ReadProjectFile(fileName);

            return ReadFileWorker(fileName);
        }

        private byte[] ReadSolutionFile(string fileName)
        {
            if (!m_RemoveSourceControl)
                return ReadFileWorker(fileName);

            using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(file))
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
            var sccSectionStart = new string[] 
            {
                "GlobalSection(SourceCodeControl)",
                "GlobalSection(TeamFoundationVersionControl)"
            };
            const string sccSectionEnd = "EndGlobalSection";

            while(!reader.EndOfStream)
            {
                string solutionLine = reader.ReadLine();
                string trimmedReadLine = TrimmedSolutionLine(solutionLine);

                if (Startswith(trimmedReadLine, sccSectionStart))
                {
                    inSccSection = true;
                }
                else if (trimmedReadLine.StartsWith(sccSectionEnd) && inSccSection)
                {
                    inSccSection = false;
                }
                else if (inSccSection)
                {
                    //This just means that nothing is returned because we are in the section.
                }
                else
                {
                    yield return solutionLine;
                }
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

        private byte[] ReadProjectFile(string fileName)
        {
            if (!m_RemoveSourceControl)
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
//            var encoding = new ASCIIEncoding();
            var encoding = new UTF8Encoding();
            byte[] content = encoding.GetBytes(doc);
            byte[] utf8Preamble = Encoding.UTF8.GetPreamble();
            byte[] newContent = new byte[content.Length + utf8Preamble.Length];
            Buffer.BlockCopy(utf8Preamble, 0, newContent, 0, utf8Preamble.Length);
            Buffer.BlockCopy(content, 0, newContent, utf8Preamble.Length, content.Length);
            return newContent; 
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
        /// This funny little thing adds a readme file called SZReadme.txt, which is an advertisement for SolZip. 
        /// It is optional of course. I do not want to force anyone to have an extra readme file. For now it is on here as an experiment.
        /// </summary>
        /// <returns></returns>
        private void AddReadme(string topPath, bool excludeSZReadme)
        {
            if (excludeSZReadme || m_SZReadmeAlreadyAdded || !m_SolutionOrProject)
                return;
            m_SZReadmeAlreadyAdded = true;
            
            Stream readmeFile = Assembly.GetExecutingAssembly().GetManifestResourceStream("SolZipBasis.readme.txt");
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
