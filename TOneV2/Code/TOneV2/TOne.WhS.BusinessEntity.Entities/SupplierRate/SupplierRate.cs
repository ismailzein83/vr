using System;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierRate : IRate, IBusinessEntity
    {
        public long SupplierRateId { get; set; }

        public long ZoneId { get; set; }

        public int PriceListId { get; set; }

        public int? CurrencyId { get; set; }

        public decimal Rate { get; set; }

        public int? RateTypeId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public string SourceId { get; set; }
        public RateChangeType RateChange { get; set; }
    }
}
