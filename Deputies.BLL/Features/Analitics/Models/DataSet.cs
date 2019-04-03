using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Analitics.Models
{
    public class DataSet
    {
        public string Title { get; set; }

        public List<double> Data { get; set; } = new List<double>();
    }
}
