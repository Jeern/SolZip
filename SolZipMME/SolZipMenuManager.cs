using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SolZipBasis;
using MMEContracts;

namespace SolZipMME
{
    public class SolZipMenuManager : IMenuManager 
    {
        public IEnumerable<IMenuItem> GetMenus(ContextLevels menuForLevel)
        {
            var menuItems = new List<IMenuItem>();
            if (menuForLevel == ContextLevels.Solution ||
                menuForLevel == ContextLevels.Project || // context.FileName.EndsWith(SolZipConstants.ProjectExtension))  
                menuForLevel == ContextLevels.Item)
            {
                var item = new MMEContracts.MenuItem("Compress with Zip");
                item.Click += MenuClicked;
                menuItems.Add(item);
            }
            return menuItems;
        }

        private void MenuClicked(object sender, EventArgs<IMenuContext> e)
        {
            var zipitForm = new SolZipMMEForm(e.Data.FullFileName);
            zipitForm.ShowDialog();
        }

        public string MainMenu()
        {
            return "SolZip";
        }
    }
}
