using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Analitics.Models
{
    public class ChartResponse
    {
        public List<string> Labels { get; set; } = new List<string>();

        public List<DataSet> Data { get; set; } = new List<DataSet>();

        public object AditionalData { get; set; }

        public object AditionalData1 { get; set; }
    }
}
