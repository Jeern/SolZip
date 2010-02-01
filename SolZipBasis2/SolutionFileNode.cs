using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SolZipBasis2
{
    public class SolutionFileNode : FileNode
    {
        public SolutionFileNode(string rootDirectory, string fileName) : base(rootDirectory, fileName) { }

        /// <summary>
        /// GetLineNumbers should return all linenumbers that this object uses in the parents 
        /// content file. These Linenumbers are exactly those that should be excluded from the 
        /// parent if the parent is excluded (if Include == false).
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override List<int> GetLineNumbersInParentContent(FileNode parent)
        {
            //A Solution file never has any parent, so this is always null
            return null;
        }

        /// <summary>
        /// The implementation of this abstract method should read through the content of the 
        /// object, and construct child nodes where relevant. E.g create  3 ProjectFileNodes, if this
        /// object is a SolutionFileNode, and it contains 3 projects.
        /// </summary>
        public override void CreateChildNodes()
        {
            var reader = new SolutionFileReader(FullFileName);
            
            IEnumerable<string> projects = reader.GetCsharpProjectItems();
            foreach (string project in projects)
            {
                AddChild(new ProjectFileNode(RootDirectory, project));
            }

            IEnumerable<string> setupProjects = reader.GetSetupProjectItems();
            foreach (string setupProject in setupProjects)
            {
                AddChild(new SetupProjectFileNode(RootDirectory, setupProject));
            }

            IEnumerable<string> items = reader.GetSolutionItems();
            foreach (string item in items)
            {
                AddChild(new ItemFileNode(RootDirectory, item));
            }
        }
    }
}
