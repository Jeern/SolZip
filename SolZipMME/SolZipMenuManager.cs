using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.AddIn;
using ManagedMenuAddInViews;
using System.Windows.Forms;
using System.IO;

namespace SolZipMME
{
    [AddIn("SolZipManagedAddIn", Version = "1.2.0.3")]
    public class SolZipMenuManager : MenuManagerAddInView 
    {
        public override List<MenuItemView> CreateMenus(MenuContextView context)
        {
            if (context.Levels == ContextLevels.Solution || context.Levels == ContextLevels.Project
                || context.Levels == ContextLevels.Item)
            {
                return new List<MenuItemView>() { new SolZipMenuItem("Compress with Zip...") };
            }
            return null;
        }

        public override string MainMenu(ApplicationTypes types)
        {
            if (types != ApplicationTypes.VS2008)
                return null;

            return "SolZip (MME)";
        }

        public override void MenuClicked(MenuItemView clickedMenu, MenuContextView menuContext)
        {
            var zipitForm = new SolZipMMEForm(menuContext.FullPath);
            zipitForm.ShowDialog();
        }
    }
}
