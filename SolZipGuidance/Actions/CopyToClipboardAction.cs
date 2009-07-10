using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.RecipeFramework;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using SolZipBasis;

namespace SolZipGuidance.Actions
{
    public class CopyToClipboardAction : ConfigurableAction
    {
        [Input]
        public string TextToCopy { get; set; }

        /// <summary>
        /// Copys the input text to the clipboard
        /// </summary>
        public override void Execute()
        {
            SolZipHelper.CopyFileNameToClipboard(TextToCopy);
        }

        public override void Undo()
        {
            //Not relevant
        }
    }
}
