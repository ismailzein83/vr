using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Business
{
    public class DealSupplierRateEvaluatorContext : IDealSupplierRateEvaluatorContext
    {
        public DateTime DealBED { get; set; }
        public DateTime? DealEED { get; set; }
        public int CurrencyId { get; set; }
        public IEnumerable<DealSupplierZoneGroupZoneItem> SupplierZoneItem { get; set; }
        public IEnumerable<DealRate> SupplierRates { get; set; }
        public Dictionary<long, SupplierRate> SupplierZoneRateByZoneId { get; set; }

    }
}
