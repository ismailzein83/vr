using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Business
{
    public class CountryManager
    {
        public IEnumerable<CountryInfo> GetCountriesInfo()
        {
            var countryReader = ReaderFactory.GetCountryReader();
            var countries = countryReader.GetCountries();
            if (countries == null)
                return null;
            else
                return countries.Select(country => new CountryInfo
                {
                    CountryId = country.CountryId,
                    Name = country.Name
                });

        }
    }
}
