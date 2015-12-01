using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.MainExtensions.CountryReaders
{
    public class VanriseCountryReader : BusinessEntity.Entities.ICountryReader
    {
        public IEnumerable<Entities.Country> GetCountries()
        {
            var countryManager = new Vanrise.Common.Business.CountryManager();
            var vrCountries = countryManager.GetAllCountries();
            if (vrCountries == null)
                return null;
            else
                return vrCountries.Select(vrCountry => MapVRCountry(vrCountry));
        }

        private static Entities.Country MapVRCountry(Vanrise.Entities.Country vrCountry)
        {
            return new Entities.Country
            {
                CountryId = vrCountry.CountryId.ToString(),
                Name = vrCountry.Name,
                SourceObject = vrCountry
            };
        }


        public Entities.Country GetCountry(string countryId)
        {
            int countryIdAsInt;
            if (!int.TryParse(countryId, out countryIdAsInt))
                throw new ArgumentException(String.Format("CountryId '{0}' is not valid integer", countryId));
            var countryManager = new Vanrise.Common.Business.CountryManager();
            var vrCountry = countryManager.GetCountry(countryIdAsInt);
            if (vrCountry == null)
                return null;
            else
                return MapVRCountry(vrCountry);
        }
    }
}
