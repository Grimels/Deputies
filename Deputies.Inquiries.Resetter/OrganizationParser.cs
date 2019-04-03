using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.Inquiries.Resetter
{
    public class Organization
    {
        public string Full { get; set; }

        public string Name { get; set; }
    }

    public static class OrganizationParser
    {
        public static Organization ParseOrganization(string str)
        {
            if (str == null)
            {
                str = "";
            }

            var token = str.Split(new string[] { " " }, StringSplitOptions.None);
            var result = "";
            var resultIndex = 0;
            for (var i = 0; i < token.Length; i++)
            {
                var interString = (result + " " + token[i]).Trim();
                if (str.IndexOf(interString) != str.LastIndexOf(interString))
                {
                    result = interString;
                }
                else
                {
                    resultIndex = i;
                    break;
                }
            }

            if (resultIndex == 0)
            {
                return new Organization()
                {
                    Full = str,
                    Name = str
                };
            }

            var boss = string.Join(" ", token.Skip(resultIndex).Take(token.Length - resultIndex * 2));
            return new Organization()
            {
                Full = result + " " + boss,
                Name = result
            };
        }
    }
}
