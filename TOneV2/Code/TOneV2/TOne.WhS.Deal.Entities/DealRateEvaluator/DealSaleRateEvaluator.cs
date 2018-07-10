using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Deal.Entities
{
    public abstract class DealSaleRateEvaluator : BaseDealRateEvaluator
    {
        public abstract void EvaluateRate(IDealSaleRateEvaluatorContext context);
    }

    public interface IDealSaleRateEvaluatorContext
    {
        int CustomerId { get; }
        int SellingProductId { get; }
        int CurrencyId { get; }
        DateTime DealBED { get; }
        DateTime? DealEED { get; }
        List<DealRate> SaleRates { get; set; }
        IEnumerable<DealSaleZoneGroupZoneItem> SaleZoneGroupItem { get; set; }
        Func<string, int, IEnumerable<SaleRateHistoryRecord>> GetCustomerZoneRatesFunc { get; }
    }
}
