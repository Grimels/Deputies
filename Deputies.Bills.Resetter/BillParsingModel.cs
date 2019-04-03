using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.Bills.Resetter
{
    class BillParsingModel
    {
        public string Title { get; set; }

        public string Number { get; set; }

        public string Date { get; set; }

        public int Session { get; set; }

        public string Included { get; set; }

        public string Rubric { get; set; }

        public string SubjectOfRight { get; set; }

        public List<string> Initiators { get; set; }

        public string MainCommittee { get; set; }

        public List<string> OtherCommittees { get; set; }

        public List<LinkParsingModel> Texts { get; set; }

        public List<LinkParsingModel> DocumentsRelatedToWork { get; set; }

        public string Url { get; set; }
    }
}
