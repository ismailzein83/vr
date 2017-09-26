using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Business
{
    public class SalePricelistFileContext : ISalePricelistFileContext
    {
        public int SellingNumberPlanId { get; set; }
        public long ProcessInstanceId { get; set; }
        public IEnumerable<int> CustomerIds { get; set; }
        public IEnumerable<SalePLZoneChange> ZoneChanges { get; set; }
        public DateTime EffectiveDate { get; set; }
        public SalePLChangeType ChangeType { get; set; }
        public IEnumerable<int> EndedCountryIds { get; set; }
        public DateTime? CountriesEndedOn { get; set; }
        public IEnumerable<NewPriceList> SalePriceLists { get; set; }
        public int CurrencyId { get; set; }
        public int UserId { get; set; }
        public IEnumerable<NewCustomerPriceListChange> CustomerPriceListChanges { get; set; }
    }
}
