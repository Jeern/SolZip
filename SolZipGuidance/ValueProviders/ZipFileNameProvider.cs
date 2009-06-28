using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolZipBasis;
using Microsoft.Practices.RecipeFramework;
using Microsoft.Practices.Common;
using System.Collections.Specialized;
using System.ComponentModel.Design;

namespace SolZipGuidance.ValueProviders
{
    /// <summary>
    /// Given a filename it suggest a filename for the ZipFile
    /// </summary>
    public class ZipFileNameProvider : ValueProvider, IAttributesConfigurable
    {
        private string m_RecipeArgumentName;

        public override bool OnBeginRecipe(object currentValue, out object newValue)
        {
            newValue = currentValue;
            IDictionaryService dictService = GetService<IDictionaryService>();
            string recipeArgumentValue = dictService.GetValue(m_RecipeArgumentName) as string;
            if (recipeArgumentValue == null)
            {
                return false;
            }
            newValue = SolZipHelper.GetZipFileName(recipeArgumentValue);
            return newValue != currentValue;
        }

        #region IAttributesConfigurable Members

        public void Configure(StringDictionary attributes)
        {
            m_RecipeArgumentName = attributes["RecipeArgument"];
        }

        #endregion
    }
}
