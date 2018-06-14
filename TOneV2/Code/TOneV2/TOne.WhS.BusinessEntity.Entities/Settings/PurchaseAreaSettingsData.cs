using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class PurchaseAreaSettingsData : Vanrise.Entities.SettingData
    {
        public int EffectiveDateDayOffset { get; set; }
        public int RetroactiveDayOffset { get; set; }
        public decimal MaximumRate { get; set; }
        public long MaximumCodeRange { get; set; }
        public List<PricelistTypeMapping> PricelistTypeMappingList { get; set; }
    }

    public class PricelistTypeMapping
    {
        public string Subject { get; set; }
        public SupplierPricelistType PricelistType { get; set; } // when remove enum class change SupplierPricelistType to SupplierPriceListType
    }

    public enum SupplierPricelistType // to be removed when the reference will be added
    {
        [Description("Rate Change")]
        RateChange = 0,

        [Description("Country")]
        Country = 1,

        [Description("Full")]
        Full = 2
    }
}
