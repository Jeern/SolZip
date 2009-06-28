using System;
using System.Collections.Generic;
using System.Text;
//using Microsoft.Practices.ComponentModel;
using Microsoft.Practices.RecipeFramework;
using EnvDTE;
using System.IO;
using Microsoft.Practices.RecipeFramework.Library;
using EnvDTE80;
//using System.Windows.Forms;
using SolutionZipper;

namespace SolZipGuidance.Actions
{
    public class ZipAction : ConfigurableAction
    {
        [Input(Required = true)]
        public string ZipFileName { get; set; }

        [Input(Required = true)]
        public string FileToZip { get; set; }

        [Input(Required = true)]
        public bool ExcludeReadme { get; set; }

        [Input(Required = true)]
        public bool RemoveSourceControl { get; set; }
        
        // Methods
        public override void Execute()
        {
            SolZipHelper.Zip(ZipFileName, FileToZip, ExcludeReadme, RemoveSourceControl);
        }

        public override void Undo()
        {
            //Not implemented
        }
    }
}
