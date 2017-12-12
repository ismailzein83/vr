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
        SalePriceListOwnerType OwnerType { get; }

        int OwnerId { get; }

        int CurrencyId { get; }

        int LongPrecisionValue { get; }

        IEnumerable<RateToChange> RatesToChange { get; }

        IEnumerable<RateToClose> RatesToClose { get; }

        IEnumerable<ExistingRate> ExistingRates { get; }

        IEnumerable<ExistingZone> ExistingZones { get; }

        IEnumerable<ExistingCustomerCountry> ExplicitlyChangedExistingCustomerCountries { get; }

        InheritedRatesByZoneId InheritedRatesByZoneId { get; }

        IEnumerable<NewRate> NewRates { set; }

        IEnumerable<ChangedRate> ChangedRates { set; }
    }
}
