﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Xml.Linq;

namespace SolutionZipper
{
    public class SolZipController : IDisposable
    {
        private FileStream m_FileStream;
        private ZipOutputStream m_ZipStream;
        private ZipEntryFactory m_ZipEntryFactory;
        private string m_TopPath; //The Path below which we will Zip (and nowhere else)

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

        public SolZipController(string zipFileName)
        {
            m_TopPath = string.Empty;
            m_FileStream = new FileStream(zipFileName, FileMode.CreateNew, FileAccess.Write);
            m_ZipStream = new ZipOutputStream(m_FileStream);
            m_ZipEntryFactory = new ZipEntryFactory(ZipEntryFactory.TimeSetting.CreateTimeUtc);
        }

        public void ZipSolution(string solutionFile)
        {
            SetTopPathFromFile(solutionFile); 
        }

        public void ZipProject(string projectFile)
        {
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
            //We will only do it if this file is under the Top folder.
            if (fileName.StartsWith(BeginingOfPath))
            {
                byte[] fileArray = ReadFile(fileName);
                ZipEntry entry = m_ZipEntryFactory.MakeFileEntry(ZipEntry.CleanName(ZipEntryFileName(fileName)));
                entry.Size = fileArray.Length;
                m_ZipStream.PutNextEntry(entry);
                m_ZipStream.Write(fileArray, 0, fileArray.Length);
            }
        }

        public byte[] ReadFile(string fileName)
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
            //m_ZipStream.Flush();
            //m_ZipStream.Finish(); 
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
