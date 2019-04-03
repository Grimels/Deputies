using Deputies.BLL.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Inquiries.Models
{
    public class InquiryFilterModel
    {
        public bool Ind { get; set; } = true;

        public bool Col { get; set; } = true;

        public bool Done { get; set; } = true;

        public bool Proc { get; set; } = true;

        public string Query { get; set; }

        public List<CheckBoxModel> Fractions { get; set; } = new List<CheckBoxModel>();

        public List<CheckBoxModel> Sessions { get; set; } = new List<CheckBoxModel>();

        public List<CheckBoxModel> Problems { get; set; } = new List<CheckBoxModel>();
    }
}
