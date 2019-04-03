using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.Infrastructure
{
    public static class DateParser
    {
        private static Dictionary<string, int> Months = new Dictionary<string, int>()
        {
            {"грудня", 12},
            {"сiчня", 1},
            {"лютого", 2},
            {"березня", 3},
            {"квiтня", 4},
            {"травня", 5},
            {"червня", 6},
            {"липня", 7},
            {"серпня", 8},
            {"вересня", 9},
            {"жовтня", 10},
            {"листопада", 11},
        };

        public static DateTime ParseLongDate(string value)
        {
            var tokens = value.Split(new string[] { " " }, StringSplitOptions.None);
            var day = int.Parse(tokens[0]);
            var month = Months[tokens[1]];
            var year = int.Parse(tokens[2]);
            return new DateTime(year, month, day).ToLocalTime();
        }

        public static DateTime? ParseShortDate(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var tokens = value.Split(new string[] { "." }, StringSplitOptions.None);
            var day = int.Parse(tokens[0]);
            var month = int.Parse(tokens[1]);
            var year = int.Parse(tokens[2]);
            return new DateTime(year, month, day).ToLocalTime();
        }
    }
}
