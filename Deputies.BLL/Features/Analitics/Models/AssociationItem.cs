using Deputies.BLL.Features.Deputies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Analitics.Models
{
    public class AssociationItem
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int Deputies { get; set; }

        public int Inquries { get; set; }

        public int Place { get; set; }

        public double InquriesPerDeputy { get; set; }

        public List<DeputyModel> TopDeputies { get; set; }
    }
}
