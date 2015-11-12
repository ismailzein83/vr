using System.Collections.Generic;
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
