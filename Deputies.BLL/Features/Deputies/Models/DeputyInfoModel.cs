using Deputies.BLL.Features.Analitics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Deputies.Models
{
    public class DeputyInfoModel
    {
        public DeputyModel DeputyModel { get; set; }

        public List<OrganizationsResponseItem> Organizations { get; set; } = new List<OrganizationsResponseItem>();
    }
}
