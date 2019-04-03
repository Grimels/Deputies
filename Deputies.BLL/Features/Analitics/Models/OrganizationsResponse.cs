using Deputies.BLL.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Analitics.Models
{
    public class OrganizationsResponse
    {
        public List<OrganizationsResponseItem> Items { get; set; }

        public PagingModel PagingModel { get; set; }
    }
}
