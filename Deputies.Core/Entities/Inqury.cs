using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParliamentaryInquiry.Core.Entities
{
    public class Inqury : BaseEntity
    {
        public string RequestNumber { get; set; }

        public string SessionId { get; set; }

        public string AuthorId { get; set; }

        public List<string> CoauthorIds { get; set; } = new List<string>();

        public string DestinationId { get; set; }

        public string ProblemId { get; set; }

        public string Body { get; set; }

        public string BodyUrl { get; set; }

        public string DateRaw { get; set; }

        public DateTime Date { get; set; }

        public string DeadlineRaw { get; set; }

        public DateTime Deadline { get; set; }

        public List<InquryAnswer> InquryAnswers { get; set; } = new List<InquryAnswer>();
    }
}
