using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
    public class RateBulkActionCorrectedData : BulkActionCorrectedData
    {
        public List<ZoneCorrectedRate> ZoneCorrectedRates { get; set; }
    }

    public class ZoneCorrectedRate
    {
        public long ZoneId { get; set; }
        public decimal CorrectedRate { get; set; }
    }
}
