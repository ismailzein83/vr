using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class SoldCountryManager
    {
        #region Fields

        private CountryManager _countryManager = new Vanrise.Common.Business.CountryManager();

        #endregion

        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<SoldCountryDetail> GetFilteredSoldCountries(Vanrise.Entities.DataRetrievalInput<SoldCountryQuery> input)
        {
            IEnumerable<CustomerCountry2> soldCountries = null;
            IEnumerable<CustomerCountry2> allCountries = new CustomerCountryManager().GetSoldCountries(input.Query.CustomerId, input.Query.EffectiveOn);
            if (allCountries != null)
            {
                if (string.IsNullOrWhiteSpace(input.Query.CountryName))
                    soldCountries = allCountries;
                else
                {
                    IEnumerable<int> countryIds = new Vanrise.Common.Business.CountryManager().GetCountryIdsBySubstring(input.Query.CountryName);
                    if (countryIds != null && countryIds.Count() > 0)
                        soldCountries = allCountries.FindAllRecords(x => countryIds.Contains(x.CountryId));
                }
            }
            Vanrise.Entities.BigResult<SoldCountryDetail> bigResult = (soldCountries != null) ? soldCountries.ToBigResult(input, null, SoldCountryDetailMapper) : null;
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, bigResult);
        }

        #endregion

        #region Mappers

        private SoldCountryDetail SoldCountryDetailMapper(CustomerCountry2 customerCountry)
        {
            return new SoldCountryDetail()
            {
                Entity = new SoldCountry()
                {
                    CountryId = customerCountry.CountryId,
                    Name = _countryManager.GetCountryName(customerCountry.CountryId),
                    BED = customerCountry.BED,
                    EED = customerCountry.EED
                },
                IsSoldInFuture = (customerCountry.BED > DateTime.Now.Date)
            };
        }

        #endregion
    }
}
