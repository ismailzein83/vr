using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Business
{
    public class DealSaleRateEvaluatorContext : IDealSaleRateEvaluatorContext
    {
        public int CustomerId { get; set; }
        public int SellingProductId { get; set; }
        public DateTime DealBED { get; set; }
        public DateTime? DealEED { get; set; }
        public int CurrencyId { get; set; }
        public List<long> ZoneIds { get; set; }
        public Dictionary<long, List<DealRate>> SaleRatesByZoneId { get; set; }
        public Func<string, int, IEnumerable<SaleRateHistoryRecord>> GetCustomerZoneRatesFunc { get; set; }
    }
}
