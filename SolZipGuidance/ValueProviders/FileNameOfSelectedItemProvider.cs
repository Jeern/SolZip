using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Common;
using Microsoft.Practices.RecipeFramework;
using Microsoft.Practices.ComponentModel;
using EnvDTE;
using EnvDTE80;

namespace SolZipGuidance.ValueProviders
{
    /// <summary>
    /// Finds the full filename of the selected item. Based on
    /// Microsoft.Practices.RecipeFramework.Extensions.ValueProviders.VisualStudio.SelectedItemPathProvider
    /// which is unfortunately not robust enough for this purpose, but thank you to the GEL project anyway :O).
    /// </summary>
    [ServiceDependency(typeof(DTE))]
    public class FileNameOfSelectedItemProvider : ValueProvider
    {
        public override bool OnBeginRecipe(object currentValue, out object newValue)
        {
            var vs = GetService(typeof(DTE)) as DTE2;
            UIHierarchyItem selectedItem = SelectedItem(vs);
            var solution = selectedItem.Object as Solution;
            if (solution != null)
            {
                newValue = solution.FileName;
                return true;
            }
            else
            {
                var project = GetProject(selectedItem.Object);
                if (project != null)
                {
                    newValue = project.FileName;
                    return true;
                }
                else
                {


                    var item = selectedItem.Object as ProjectItem;
                    if (item != null)
                    {
                        newValue = item.get_FileNames(1);
                        return true;
                    }
                }
            }
            newValue = currentValue;
            return false;
        }

        private UIHierarchyItem SelectedItem(DTE2 vs)
        {
            if (vs == null)
                return null;

            UIHierarchy uiHierarchy = vs.ToolWindows.SolutionExplorer;
            if (uiHierarchy == null)
                return null;

            object[] items = uiHierarchy.SelectedItems as object[];
            if (items == null || items.Length == 0)
                return null;

            return items[0] as UIHierarchyItem;
        }

        /// <summary>
        /// Returns af project. Because of problems with Projects that are subprojects of solution folders
        /// it is not as easy as casting. Instead we must do some more magic.
        /// </summary>
        /// <param name="selectedItemObject"></param>
        /// <returns></returns>
        private Project GetProject(object selectedItemObject)
        {
            var project = selectedItemObject as Project;
            if (project != null)
                return project;

            var item = selectedItemObject as ProjectItem;
            if (item == null)
                return null;

            return item.SubProject;
        }
    }
}
