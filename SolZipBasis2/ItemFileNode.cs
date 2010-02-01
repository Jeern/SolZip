using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolZipBasis2
{
    public class ItemFileNode : FileNode
    {
        public ItemFileNode(string rootDirectory, string fileName) : base(rootDirectory, fileName) { }
        public ItemFileNode(string rootDirectory, string fileName, FileNode parent) : base(rootDirectory, fileName, parent) { }

        /// <summary>
        /// GetLineNumbers should return all linenumbers that this object uses in the parents 
        /// content file. These Linenumbers are exactly those that should be excluded from the 
        /// parent if the parent is excluded (Include == false).
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override List<int> GetLineNumbers(FileNode parent)
        {
            if (parent is SolutionFileNode)
            {
                return new List<int>() { GetLineNumberSolutionFile(parent) };
            }
            return null;
        }

        private int GetLineNumberSolutionFile(FileNode parent)
        {
            return
                parent.GetFirstLineNumberContaining(FileName,
                    parent.GetFirstLineNumberContaining("ProjectSection(SolutionItems)")+1);
        }

        /// <summary>
        /// The implementation of this abstract method should read through the content of the 
        /// object, and construct child nodes where relevant. The ItemFileNode has no children
        /// and therefore ReadChildren is not doing anything.
        /// </summary>
        public override void ReadChildren()
        {
            //Not used for ItemFileNode
        }


    }
}
