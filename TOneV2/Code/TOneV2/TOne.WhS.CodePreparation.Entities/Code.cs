using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities
{
    public class Code
    {
        public long SaleCodeId { get; set; }

        public string CodeValue { get; set; }

        public long ZoneId { get; set; }
        public int CodeGroupId { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }

        public Status Status { get; set; }
    }
}
