using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class CountryWithDefaultRateManager 
    {

        public Vanrise.Entities.IDataRetrievalResult<CountryDetail> GetFilteredCountries(Vanrise.Entities.DataRetrievalInput<CountryWithDefaultRateQuery> input)
        {
            CountryManager countryManager = new CountryManager();
            RatePlanDraftManager ratePlanDraftManager = new RatePlanDraftManager();
            IEnumerable<Vanrise.Entities.Country> allCountries = countryManager.GetAllCountries();
            var countriesWithDefaultRates = ratePlanDraftManager.GetSellingZonesWithDefaultRatesTaskData(input.Query.OwnerType, input.Query.OwnerId).ZoneIdsWithDefaultRatesByCountryIds.Keys.ToList<int>();
            Func<Vanrise.Entities.Country, bool> filterExpression = (prod) =>
          (countriesWithDefaultRates.Contains(prod.CountryId));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCountries.ToBigResult(input, filterExpression, CountryDetailMapper), null);
        }


        private CountryDetail CountryDetailMapper(Vanrise.Entities.Country country)
        {
            CountryDetail countryDetail = new CountryDetail();

            countryDetail.Entity = country;
            return countryDetail;
        }
    }
}
