using ParliamentaryInquiry.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.Core.Entities
{
    public class NotificationItem : BaseEntity
    {
        public string Email { get; set; }

        public string DeputyId { get; set; }
    }
}
