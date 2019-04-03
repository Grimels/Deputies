using Deputies.BLL.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Inquiries.Models
{
    public class InquriesListModel
    {
        public IList<InquiryModel> Inquries { get; set; }

        public PagingModel PagingModel { get; set; }

        public InquiryFilterModel FilterModel { get; set; }

        public int Count { get; set; }

        public bool Asc;

        public string DeputyName { get; set; }

        public string DeputyId { get; set; }

        public string Destination { get; set; }
    }
}
