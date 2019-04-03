using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParliamentaryInquiry.Core.Entities
{
    public class DeputyAssociation : BaseEntity
    {
        public string Name { get; set; }

        public DeputyAssociationType Type { get; set; }
    }
}
