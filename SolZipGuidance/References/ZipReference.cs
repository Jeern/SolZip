using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.RecipeFramework;
using System.Runtime.Serialization;
using EnvDTE;

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
            //Skal give true hvis det markerede er en Solution, Project eller Item
            var solution = target as Solution;
            var project = target as Project;
            var item = target as UIHierarchyItem;

            if (solution != null || project != null || item != null)
                return true;

            return false;
        }

        public override string AppliesTo
        {
            get { return "Solutions, C# Projects, Setup projects and Items"; }
        }
    }
}
