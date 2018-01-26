using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public interface IProcessRatesContext
    {
        #region Input Properties
        long ProcessInstanceId { get; }
        int UserId { get; }
        SalePriceListOwnerType OwnerType { get; }
        int OwnerId { get; }
        DateTime PriceListCreationDate { get; }
        int CurrencyId { get; }
        int LongPrecisionValue { get; }
        int ReservedPriceListId { get; }
        IEnumerable<RateToChange> RatesToChange { get; }
        IEnumerable<RateToClose> RatesToClose { get; }
        IEnumerable<ExistingRate> ExistingRates { get; }
        IEnumerable<ExistingZone> ExistingZones { get; }
        IEnumerable<ExistingCustomerCountry> ExplicitlyChangedExistingCustomerCountries { get; }
        InheritedRatesByZoneId InheritedRatesByZoneId { get; }
        #endregion

        #region Output Properties
        IEnumerable<NewRate> OwnerNewRates { set; }
        IEnumerable<NewRate> NewRatesToFillGapsDueToClosingCountry { set; }
        IEnumerable<NewRate> NewRatesToFillGapsDueToChangeSellingProductRates { set; }
        IEnumerable<ChangedRate> ChangedRates { set; }
        Dictionary<int, List<NewPriceList>> CustomerPriceListsByCurrencyId { set; }
        #endregion
    }
}
