using Deputies.BLL.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Bills.Models
{
    public class BillsListModel
    {
        public IList<BillModel> Bills { get; set; }

        public PagingModel PagingModel { get; set; }

        public int Count { get; set; }

        public bool Asc;

        public BillFilterModel FilterModel { get; set; }

        public string DeputyName { get; set; }

        public string DeputyId { get; set; }
    }
}
