using Deputies.BLL.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deputies.BLL.Features.Deputies.Models
{
    public class DeputyModel : BaseModel
    {
        public string Name { get; set; }

        public string Position { get; set; }

        public string AssociationId { get; set; }

        public string Association { get; set; } = "";

        public string Link { get; set; }

        public string PhotoLink { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }

        public int InquriesCount { get; set; } = 0;

        public string ChosenBy { get; set; } = "";

        public string Party { get; set; } = "";
    }
}
