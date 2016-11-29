using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public interface IProcessRatesContext
    {
        IEnumerable<RateToChange> RatesToChange { get; }

        IEnumerable<RateToClose> RatesToClose { get; }

        IEnumerable<ExistingRate> ExistingRates { get; }

        IEnumerable<ExistingZone> ExistingZones { get; }

		IEnumerable<ExistingCustomerCountry> ExplicitlyChangedExistingCustomerCountries { get; }

        IEnumerable<NewRate> NewRates { set; }

        IEnumerable<ChangedRate> ChangedRates { set; }
    }
}
