using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParliamentaryInquiry.Core.Entities
{
    public class SingleMemberDeputy : Deputy
    {
        public string ConstituencyId { get; set; }
    }
}
