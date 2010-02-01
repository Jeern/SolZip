using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolZipBasis2
{
    public class SetupProjectFileNode : FileNode
    {
        public SetupProjectFileNode(string rootDirectory, string fileName) : base(rootDirectory, fileName) { }
        public SetupProjectFileNode(string rootDirectory, string fileName, FileNode parent) : base(rootDirectory, fileName, parent) { }

        /// <summary>
        /// GetLineNumbers should return all linenumbers that this object uses in the parents 
        /// content file. These Linenumbers are exactly those that should be excluded from the 
        /// parent if the parent is excluded (Include == false).
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override List<int> GetLineNumbersInParentContent(FileNode parent)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The implementation of this abstract method should read through the content of the 
        /// object, and construct child nodes where relevant. E.g create a 3 ProjectFileNodes, if this
        /// object is a SolutionFileNode, and it contains 3 projects.
        /// </summary>
        public override void CreateChildNodes()
        {
            throw new NotImplementedException();
        }
    }
}
