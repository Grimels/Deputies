using ParliamentaryInquiry.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Shared.Models
{
    public class DeputyAssociationModel
    {
        public string Name { get; set; }

        public DeputyAssociationType Type { get; set; }
    }
}
