using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SolZipBasis2
{
    public abstract class FileNode
    {
        public FileNode(string rootDirectory, string fileName) : this(rootDirectory, fileName, null) { }
        public FileNode(string rootDirectory, string fileName, FileNode parent)
        {
            Include = true;
            m_FileName = fileName;
            m_RootDirectory = rootDirectory;
            m_Parent = parent;
            if (parent != null)
            {
                parent.AddChild(this);
            }
            CreateChildNodes();
        }

        public string FullFileName 
        {
            get { return Path.Combine(m_RootDirectory, m_FileName); } 
        }

        private string m_FileName;
        public string FileName
        {
            get { return m_FileName; }
        }

        private string m_RootDirectory;
        public string RootDirectory
        {
            get { return m_RootDirectory; }
        }

        public ModifiedFileStream Content
        {
            get { return null; }
        }

        public bool Include { get; set; }

        private FileNode m_Parent;
        public FileNode Parent 
        {
            get { return m_Parent; } 
        }

        private List<FileNode> m_Children = new List<FileNode>();
        public List<FileNode> Children
        {
            get { return m_Children; }
        }

        /// <summary>
        /// Adds a child to the children collection
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(FileNode child)
        {
            m_Children.Add(child);
        }

        /// <summary>
        /// Returns all line numbers that are to be excluded from this files content
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> ExcludeLineNumbers()
        {
            return
                from node in Children.SelectMany(child => child.GetLineNumbersInParentContent(this),
                    (child, lineNumber) => new { child.Include, lineNumber })
                where !node.Include
                select node.lineNumber;                
        }


        /// <summary>
        /// GetLineNumbers should return all linenumbers that this object uses in the parents 
        /// content file. These Linenumbers are exactly those that should be excluded from the 
        /// parent if the parent is excluded (Include == false).
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public abstract List<int> GetLineNumbersInParentContent(FileNode parent);

        /// <summary>
        /// The implementation of this abstract method should read through the content of the 
        /// object, and construct child nodes where relevant. E.g create a 3 ProjectFileNodes, if this
        /// object is a SolutionFileNode, and it contains 3 projects.
        /// </summary>
        public abstract void CreateChildNodes();

        private List<string> m_ContentLines;

        internal List<string> ContentLines
        {
            get
            {
                if (m_ContentLines == null)
                {
                    m_ContentLines = GetAllLinesWorker().ToList();
                }
                return m_ContentLines;
            }
        }

        private IEnumerable<string> GetAllLinesWorker()
        {
            using (var fileStream = new FileStream(FullFileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    while (!reader.EndOfStream)
                    {
                        yield return reader.ReadLine();
                    }
                }
            }
        }

        /// <summary>
        /// Returns the first linenumber of the ContentLines containing searchstring. Lines start at 0.
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="startPosition"></param>
        /// <returns></returns>
        internal int GetFirstLineNumberContaining(string searchString)
        {
            return GetFirstLineNumberContaining(searchString, 0);
        }

        /// <summary>
        /// Returns the first linenumber of the ContentLines containing searchstring. Lines start at 0.
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="startPosition"></param>
        /// <returns></returns>
        internal int GetFirstLineNumberContaining(string searchString, int startPosition)
        {
            return SolZipHelper.GetLineNumbersContaining(ContentLines, searchString, startPosition).First();
        }
    }
}
