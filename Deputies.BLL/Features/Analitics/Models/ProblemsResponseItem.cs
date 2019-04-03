using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Analitics.Models
{
    public class ProblemsResponseItem
    {
        public string Name { get; set; }

        public int Count { get; set; }

        public double Percent { get; set; }
    }
}
