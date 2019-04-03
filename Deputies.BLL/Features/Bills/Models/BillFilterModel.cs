using Deputies.BLL.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Bills.Models
{
    public class BillFilterModel
    {
        public string Query { get; set; }

        public List<CheckBoxModel> Sessions { get; set; } = new List<CheckBoxModel>();

        public List<CheckBoxModel> Rubrics { get; set; } = new List<CheckBoxModel>();

        public List<CheckBoxModel> SubjectsOfRight { get; set; } = new List<CheckBoxModel>();
    }
}
