using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities.RatePlanning
{
    public class ZoneItem
    {
        public long ZoneId { get; set; }

        public string ZoneName { get; set; }

        public long? CurrentRateId { get; set; }

        public Decimal? CurrentRate { get; set; }

        public bool IsCurrentRateEditable { get; set; }

        public Decimal? NewRate { get; set; }

        public DateTime? RateBED { get; set; }

        public DateTime? RateEED { get; set; }
    }
}
