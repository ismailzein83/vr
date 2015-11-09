using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;


namespace Vanrise.Common.Data
{
    public interface ICountrytDataManager : IDataManager
    {
        List<Country> GetCountries();
        bool Update(Country country);
        bool Insert(Country country, out int insertedId);
        bool AreCountriesUpdated(ref object updateHandle);
       
    }
}
