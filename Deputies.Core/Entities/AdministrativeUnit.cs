using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParliamentaryInquiry.Core.Entities
{
    public class AdministrativeUnit : BaseEntity
    {
        public string Name { get; set; }

        public string ParrentId { get; set; }
    }
}
