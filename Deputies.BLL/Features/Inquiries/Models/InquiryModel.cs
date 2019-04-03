using Deputies.BLL.Features.Deputies.Models;
using Deputies.BLL.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Inquiries.Models
{
    public class InquiryModel : BaseModel
    {
        public string RequestNumber { get; set; }

        public string Session { get; set; }

        public string Author { get; set; }

        public string AuthorId { get; set; }

        public string Destination { get; set; }

        public string Problem { get; set; }

        public string Body { get; set; }

        public string BodyUrl { get; set; }

        public string DateRaw { get; set; }

        public string DeadlineRaw { get; set; }

        public List<string> CoauthorIds { get; set; } = new List<string>();

        public List<DeputyModel> Coauthors { get; set; } = new List<DeputyModel>();

        public List<InquiryAnswerModel> InquryAnswers { get; set; } = new List<InquiryAnswerModel>();
    }
}
