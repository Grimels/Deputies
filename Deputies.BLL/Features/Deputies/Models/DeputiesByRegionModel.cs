using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Deputies.Models
{
    public class DeputiesByRegionModel
    {
        public string RegionName { get; set; }

        public List<DeputyByConstituencyModel> ByRegion { get; set; }

        public List<DeputiesByRegionModel> BySubregion { get; set; }

        public DeputiesByRegionModel()
        {
            this.ByRegion = new List<DeputyByConstituencyModel>();
            this.BySubregion = new List<DeputiesByRegionModel>();
        }
    }
}
