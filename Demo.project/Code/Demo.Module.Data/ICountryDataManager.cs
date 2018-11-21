using Demo.Module.Entities;
using System.Collections.Generic;

namespace Demo.Module.Data
{
	public interface ICountryDataManager : IDataManager
	{
		List<Country> GetCountries();

		bool Insert(Country country, out int insertedId);

		bool Update(Country country);

		bool AreCountryUpdated(ref object updateHandle);
	}
}