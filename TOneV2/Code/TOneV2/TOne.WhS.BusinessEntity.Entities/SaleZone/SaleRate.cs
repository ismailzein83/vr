using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleRate : IRate, Vanrise.Entities.IDateEffectiveSettings
    {
        public long SaleRateId { get; set; }

        public long ZoneId { get; set; }

        public int PriceListId { get; set; }

        public int? CurrencyId { get; set; }

        public decimal NormalRate { get; set; }

        public Dictionary<int, decimal> OtherRates { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class SaleRatePriceList
    {
        public SaleRate Rate { get; set; }

        public SalePriceList PriceList { get; set; }
    }
}
