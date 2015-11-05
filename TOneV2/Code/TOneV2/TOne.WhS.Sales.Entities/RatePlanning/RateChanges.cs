using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities.RatePlanning
{
    public class RateChanges : BaseChanges
    {
        public List<NewRate> NewRates { get; set; }

        public List<RateChange> ChangedRates { get; set; }
    }

    public class NewRate
    {
        public long ZoneId { get; set; }

        public Decimal NormalRate { get; set; }

        public Dictionary<int, Decimal> OtherRates { get; set; }

        public int? CurrencyId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class RateChange
    {
        public long RateId { get; set; }

        public DateTime? EED { get; set; }
    }
}
