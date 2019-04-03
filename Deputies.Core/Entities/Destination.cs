using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParliamentaryInquiry.Core.Entities
{
    public class Destination : BaseEntity
    {
        public string NameRaw { get; set; }

        public string Name { get; set; }

        public string Full { get; set; }
    }
}
