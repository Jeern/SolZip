using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

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
        public override List<int> GetLineNumbersInParentContent(FileNode parent)
        {
            if (parent is SolutionFileNode)
            {
                return new List<int>() { GetLineNumberSolutionFile(parent) };
            }

            if (parent is ProjectFileNode)
            {
                return GetLineNumbersProjectFile(parent);
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
        /// Returns all the Line numbers in the projectfile that corresponds to this fileitem.
        /// Normally it will only be one line. But sometimes the XML stretches several lines.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private List<int> GetLineNumbersProjectFile(FileNode parent)
        {
            
            XDocument doc = XDocument.Load(FullFileName, LoadOptions.SetLineInfo);
            //First find the XML Element.
            var element =
                (from attr in doc.Descendants().Attributes("Include")
                 where attr.Value == FileName
                 select attr.Parent).First();

            if(!element.HasElements)
            {
                //In this case the element has now "subelements" which means it spans only one
                //line.
                return new List<int>() { element.GetLineNumber() };
            }

            //Now we know there is a Sub element. Which means we need to span more lines
            int firstLine = element.GetLineNumber();
            int lastLine = element.NextNode.GetLineNumber(); // This is the linenumber of the END element


            return SolZipHelper.GetListOfIntegers(firstLine, lastLine).ToList();
        }

        

        /// <summary>
        /// The implementation of this abstract method should read through the content of the 
        /// object, and construct child nodes where relevant. The ItemFileNode has no children
        /// and therefore ReadChildren is not doing anything.
        /// </summary>
        public override void CreateChildNodes()
        {
            //Not used for ItemFileNode
        }


    }
}
