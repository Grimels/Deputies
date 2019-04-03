using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.Infrastructure
{
    public static class Extentions
    {
        public static string TrimEndString(this string input, string suffixToRemove, StringComparison comparisonType)
        {

            if (input != null && suffixToRemove != null
              && input.EndsWith(suffixToRemove, comparisonType))
            {
                return input.Substring(0, input.Length - suffixToRemove.Length);
            }
            else return input;
        }
    }
}
