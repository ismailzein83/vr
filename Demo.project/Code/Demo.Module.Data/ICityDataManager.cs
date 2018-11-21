using Demo.Module.Entities;
using System.Collections.Generic;

namespace Demo.Module.Data
{
	public interface ICityDataManager : IDataManager
	{
		List<City> GetCities();

		bool Insert(City city, out int insertedId);

		bool Update(City city);

		bool AreCityUpdated(ref object updateHandle);
	}
}