using ParliamentaryInquiry.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.Core.Entities
{
    public class Bill : BaseEntity
    {
        public string Title { get; set; }

        public string Number { get; set; }

        public string DateRaw { get; set; }

        public DateTime Date { get; set; }

        public string SessionId { get; set; }

        public string Included { get; set; }

        public string RubricId { get; set; }

        public string SubjectOfRightId { get; set; }

        public List<string> InitiatorsIds { get; set; } = new List<string>();

        public string MainCommitteeId { get; set; }

        public List<string> OtherCommitteesIds { get; set; } = new List<string>();

        public List<Link> Texts { get; set; }

        public List<Link> DocumentsRelatedToWork { get; set; }
    }
}
