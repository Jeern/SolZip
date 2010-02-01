using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace SolZipBasis2
{
    public static class SolZipExtensions
    {
        public static IEnumerable<string> UnionDistinct(this IEnumerable<string> items1, IEnumerable<string> items2)
        {
            IEnumerable<string> distinct1 = items1.Distinct();
            IEnumerable<string> distinct2 = items2.Distinct();

            if (distinct1 != null)
            {
                foreach (string item in distinct1)
                {
                    yield return item;
                }
            }
            if (distinct2 != null)
            {
                foreach (string item in distinct2)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Returns the LineNumber of the XElement if no linenumber throws exception
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static int GetLineNumber(this XNode node)
        {
            var lineInfo = node as IXmlLineInfo;
            if (lineInfo == null || !lineInfo.HasLineInfo())
                throw new InvalidOperationException("There is no line information on the XNode: " + node.ToString());

            return lineInfo.LineNumber;
        }

    }
}
