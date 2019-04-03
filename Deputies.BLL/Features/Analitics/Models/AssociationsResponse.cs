using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Analitics.Models
{
    public class AssociationsResponse
    {
        public List<AssociationItem> Associations { get; set; } = new List<AssociationItem>();

        public int MaxDeputies { get; set; }

        public int MaxInquries { get; set; }

        public double MaxInquriesPerDeputy { get; set; }
    }
}
