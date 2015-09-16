using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class Code
    {
        public long ID { get; set; }

        public int ZoneId { get; set; }

        public string Value { get; set; }

        public Nullable<DateTime> BeginEffectiveDate { get; set; }

        public Nullable<DateTime> EndEffectiveDate { get; set; }

        public string CodeGroup { get; set; }

        public string SupplierId { get; set; }

        public byte[] Timestamp { get; set; }
        public int ServicesFlag { get; set; }
        public string Name { get; set; }
    }
}
