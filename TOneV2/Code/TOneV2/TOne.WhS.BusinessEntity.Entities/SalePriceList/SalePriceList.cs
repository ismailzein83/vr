using System;
using System.ComponentModel;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum SalePriceListOwnerType : byte
    {
        [Description("Selling Product")]
        SellingProduct = 0,
        [Description("Customer")]
        Customer = 1
    }

    public enum SalePriceListType
    {
        Full = 0,
        Country = 1,
        RateChange = 2
    }

    public class SalePriceList
    {
        public int PriceListId { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }

        public SalePriceListType PriceListType { get; set; }

        public int OwnerId { get; set; }

        public int CurrencyId { get; set; }

        public string SourceId { get; set; }

        public DateTime EffectiveOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
