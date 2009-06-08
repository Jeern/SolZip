using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.RecipeFramework;
using System.IO;
using System.Diagnostics;

namespace SolZipGuidance.Actions
{
    public class ShowExplorerAction : ConfigurableAction
    {
        [Input]
        public string Folder { get; set; }

        /// <summary>
        /// Shows the explorer in the given folder.
        /// It is very forgiven. If we provide a file it opens the folder in which the file is placed.
        /// </summary>
        public override void Execute()
        {
            if (File.Exists(Folder)) //It is really a file
            {
                Folder = Path.GetDirectoryName(Folder);
            }
            if (!Directory.Exists(Folder))
                throw new ArgumentException("You must provide a real folder name for ShowExplorerAction", "Folder");

            Process.Start(Folder);
        }

        public override void Undo()
        {
            //Not relevant
        }
    }
}
