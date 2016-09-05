using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleRate : IRate, Vanrise.Entities.IDateEffectiveSettings
    {
        public long SaleRateId { get; set; }

        public long ZoneId { get; set; }

        public int PriceListId { get; set; }

        public int? CurrencyId { get; set; }

        public int? RateTypeId { get; set; }

        public decimal NormalRate { get; set; }

        public Dictionary<int, decimal> OtherRates { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public string SourceId { get; set; }
        public RateChangeType RateChange { get; set; }
    }

    public class SaleRatePriceList
    {
        public SaleRate Rate { get; set; }

        public Dictionary<int, SaleRate> RatesByRateType { get; set; }

        //TODO: Remove this property
        //public SalePriceList PriceList { get; set; }

        public string SourceId { get; set; }
    }

    public enum RateChangeType
    {

        [Description("Not Changed")]
        NotChanged = 0,

        [Description("New")]
        New = 1,

        [Description("Deleted")]
        Deleted = 2,

        [Description("Increase")]
        Increase = 3,

        [Description("Decrease")]
        Decrease = 4
    }
}

