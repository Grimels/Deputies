using Deputies.BLL.Features.Deputies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Analitics.Models
{
    public class DeputyRatingItem
    {
        public DeputyModel Deputy { get; set; }

        public int IndividualInquries { get; set; }

        public int CollectiveInquries { get; set; }
    }
}
