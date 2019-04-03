using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParliamentaryInquiry.Core.Entities
{
    public class InquryAnswer
    {
        public string AnswerDateRaw { get; set; }

        public string AnswerBody { get; set; }

        public string AnswerBodyUrl { get; set; }

        public string FamiliarizationDateRaw { get; set; }

        public DateTime? FamiliarizationDate { get; set; }

        public DateTime? AnswerDate { get; set; }
    }
}
