
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParliamentaryInquiry.Core.Entities
{
    public class Deputy : BaseEntity
    {
        public string Name { get; set; }

        public string Position { get; set; }

        public string AssociationId { get; set; }

        public string Link { get; set; }

        public string PhotoLink { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }
    }
}
