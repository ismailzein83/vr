using System;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleRateQuery
    {
        public DateTime EffectiveOn { get; set; }

        public List<int> CountriesIds { get; set; }

        public string SaleZoneName { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public int? SellingNumberPlanId { get; set; }

        public int? CurrencyId { get; set; }

        public bool IsSystemCurrency { get; set; }

        public List<string> ColumnsToShow { get; set; }
        public bool ByCode { get; set; }
    }
}
