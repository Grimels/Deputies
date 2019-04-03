using Deputies.BLL.Shared.Models;
using Deputies.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Bills.Models
{
    public class BillModel : BaseModel
    {
        public string Title { get; set; }

        public string Number { get; set; }

        public string DateRaw { get; set; }

        public int Session { get; set; }

        public string Included { get; set; }

        public string Rubric { get; set; }

        public string SubjectOfRight { get; set; }

        public List<Initiator> Initiators { get; set; } = new List<Initiator>();

        public string MainCommittee { get; set; }

        public List<string> OtherCommittees { get; set; } = new List<string>();

        public List<Link> Texts { get; set; }

        public List<Link> DocumentsRelatedToWork { get; set; }
    }
}
