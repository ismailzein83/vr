using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.WhS.SupplierPriceList.Entities
{
   public class Code
    {
        public long SupplierCodeId { get; set; }

        public string CodeValue { get; set; }

        public long ZoneId { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }
        public Status Status { get; set; }
        
    }
}
