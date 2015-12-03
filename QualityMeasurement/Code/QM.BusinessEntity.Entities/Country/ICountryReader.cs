using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Entities
{
    public interface ICountryReader
    {
        IEnumerable<Country> GetCountries();

        Country GetCountry(string countryId);
    }
}
