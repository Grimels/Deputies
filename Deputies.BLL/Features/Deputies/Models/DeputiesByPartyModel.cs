using Deputies.BLL.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Deputies.Models
{
    public class DeputiesByPartyModel
    {
        public PoliticalPartyModel Party { get; set; }

        public IEnumerable<DeputyModel> Deputies { get; set; }
    }
}
