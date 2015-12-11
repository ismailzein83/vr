using System.Collections.Generic;
using Vanrise.Entities;


namespace Vanrise.Common.Data
{
    public interface ICountrytDataManager : IDataManager
    {
        List<Country> GetCountries();
        bool Update(Country country);
        bool Insert(Country country);
        void InsertSynchronize(Country country);
        void UpdateSynchronize(Country country);
        bool AreCountriesUpdated(ref object updateHandle);
       
    }
}
