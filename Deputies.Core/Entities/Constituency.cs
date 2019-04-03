using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace ParliamentaryInquiry.Core.Entities
{
    [BsonIgnoreExtraElements]
    public class Constituency : BaseEntity
    {
        public int Number { get; set; }

        public string RegionId { get; set; }

        public string Boundaries { get; set; }
    }
}
