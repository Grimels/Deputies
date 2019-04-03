using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Shared.Models
{
    public class ConstituencyModel : BaseModel
    {
        public int Number { get; set; }

        public string RegionId { get; set; }

        public string Boundaries { get; set; }
    }
}
