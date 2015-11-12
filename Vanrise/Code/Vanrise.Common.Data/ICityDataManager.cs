using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface ICityDataManager : IDataManager
    {
        List<City> GetCities ();
        bool Update (City city);
        bool Insert (City city, out int insertedId);
        bool AreCitiesUpdated ( ref object updateHandle);
    }
}
