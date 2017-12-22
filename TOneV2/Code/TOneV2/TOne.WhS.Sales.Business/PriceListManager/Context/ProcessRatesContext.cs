using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class ProcessRatesContext : IProcessRatesContext
    {
        #region Input Properties
        public long ProcessInstanceId { get; set; }
        public int UserId { get; set; }
        public SalePriceListOwnerType OwnerType { get; set; }
        public int OwnerId { get; set; }
        public DateTime PriceListCreationDate { get; set; }
        public int CurrencyId { get; set; }
        public int LongPrecisionValue { get; set; }
        public int ReservedPriceListId { get; set; }
        public IEnumerable<RateToChange> RatesToChange { get; set; }
        public IEnumerable<RateToClose> RatesToClose { get; set; }
        public IEnumerable<ExistingRate> ExistingRates { get; set; }
        public IEnumerable<ExistingZone> ExistingZones { get; set; }
        public IEnumerable<ExistingCustomerCountry> ExplicitlyChangedExistingCustomerCountries { get; set; }
        public InheritedRatesByZoneId InheritedRatesByZoneId { get; set; }
        #endregion

        #region Output Properties
        public IEnumerable<NewRate> NewRates { get; set; }
        public IEnumerable<ChangedRate> ChangedRates { get; set; }
        public Dictionary<int, List<NewPriceList>> CustomerPriceListsByCurrencyId { get; set; }
        #endregion
    }
}
