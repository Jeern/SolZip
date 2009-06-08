using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.RecipeFramework;
using System.Runtime.Serialization;
using EnvDTE;
using System.IO;

namespace SolZipGuidance.References
{
    [Serializable]
    public class ZipReference : UnboundRecipeReference
    {
        public ZipReference(string recipe) : base(recipe) { }

        public ZipReference(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override bool IsEnabledFor(object target)
        {
            //Menuen skal enables hvis det markerede er en Solution, Project eller Item
            return IsSolution(target) || IsProject(target) || IsFile(target);
        }

        private bool IsSolution(object target)
        {
            var solution = target as Solution;
            return (solution != null && File.Exists(solution.FileName));
        }

        private bool IsProject(object target)
        {
            var project = target as Project;
            return (project != null && File.Exists(project.FileName));
        }

        private bool IsFile(object target)
        {
            var file = target as ProjectItem;
            string fileName = string.Empty;
            if (file != null)
            {
                try
                {
                    fileName = file.get_FileNames(1);
                }
                catch
                {
                    return false;
                }
            }
            return (file != null && File.Exists(fileName));
        }

        public override string AppliesTo
        {
            get { return "Solutions, C# Projects, Setup projects and Items"; }
        }
    }
}
