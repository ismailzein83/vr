using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public class DP
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Number { get; set; }

        public long Area { get; set; }

        public long Site { get; set; }

        public Guid Status { get; set; }

        public int Region { get; set; }

        public int City { get; set; }

        public int Town { get; set; }

        public long Street { get; set; }

        public string BuildingDetails { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedTime { get; set; }

        public int LastModifiedBy { get; set; }

        public DateTime LastModifiedTime { get; set; }
    }
}