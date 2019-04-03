using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Analitics.Models
{
    public class DeputyAnalytics
    {
        public ChartResponse Sessions { get; set; }

        public ChartResponse BillsBySessions { get; set; }

        public ChartResponse FromAll { get; set; }

        public ChartResponse FromAssociation { get; set; }

        public ChartResponse FromSameSelected { get; set; }

        public List<ProblemsResponseItem> Problems { get; set; }

        public List<ProblemsResponseItem> Rubrics { get; set; }

        public bool IsSingle { get; set; }
    }
}
