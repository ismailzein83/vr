using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SellingProductZoneRateHistoryRecord
    {
        public decimal Rate { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class SellingProductZoneRateHistoryRecordDetail
    {
        public SellingProductZoneRateHistoryRecord Entity { get; set; }
    }
}
