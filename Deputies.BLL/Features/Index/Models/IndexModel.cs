using Deputies.BLL.Features.Analitics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Index.Models
{
    public class IndexModel
    {
        public List<DeputyRatingItem> Items { get; set; }

        public AssociationsResponse Associations { get; set; }
    }
}
