using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.RecipeFramework;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

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
            Clipboard.SetText(TextToCopy);
        }

        public override void Undo()
        {
            //Not relevant
        }
    }
}
