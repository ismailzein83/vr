using System.Collections.Generic;
using Vanrise.Entities;


namespace Vanrise.Common.Data
{
    public interface ICountrytDataManager : IDataManager
    {
        List<Country> GetCountries();
        bool Update(Country country);
        bool Insert(Country country);
        void InsertFromSource(Country country);
        void UpdateFromSource(Country country);
        bool AreCountriesUpdated(ref object updateHandle);
       
    }
}
