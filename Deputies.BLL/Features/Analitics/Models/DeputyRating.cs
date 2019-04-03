using Deputies.BLL.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Analitics.Models
{
    public class DeputyRating
    {
        public List<DeputyRatingItem> Items { get; set; }

        public PagingModel PagingModel { get; set; }

        public List<CheckBoxModel> Associations { get; set; }

        public string Deps { get; set; }
    }
}
