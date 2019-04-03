using Deputies.BLL.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Deputies.Models
{
    public class DeputyByConstituencyModel
    {
        public ConstituencyModel Constituency { get; set; }

        public SingleMemberDeputyModel Deputy { get; set; }
    }
}
