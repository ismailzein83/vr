using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Entities;

namespace Demo.Module.Business
{
	public class CountryManager
	{
		#region Public Methods

		public IDataRetrievalResult<CountryDetails> GetFilteredCountries(DataRetrievalInput<Entities.CountryQuery> input)
		{
			var allCountries = GetCachedCountries();

			Func<Entities.Country, bool> filterExpression = (country) =>
			{
				if (input != null)
				{
					if (input.Query.Name != null && !country.Name.ToLower().Contains(input.Query.Name.ToLower()))
						return false;
				}

				return true;
			};

			return DataRetrievalManager.Instance.ProcessResult(input, allCountries.ToBigResult(input, filterExpression, CountryDetailMapper));
		}

		public IEnumerable<Entities.CountryInfo> GetCountryInfo(CountryInfoFilter countryInfoFilter)
		{
			var allCountries = GetCachedCountries();

			Func<Entities.Country, bool> filterFunc = (country) =>
			{
				return true;
			};
			return allCountries.MapRecords(CountryInfoMapper, filterFunc).OrderBy(country => country.Name);
		}

		public string GetCountryName(int countryId)
		{
			var country = GetCountryById(countryId);
			if (country == null)
				return null;

			return country.Name;
		}

		public Entities.Country GetCountryById(int countryId)
		{
			Dictionary<int, Entities.Country> allCountries = GetCachedCountries();
			return allCountries.GetRecord(countryId);
		}

		public InsertOperationOutput<CountryDetails> AddCountry(Entities.Country country)
		{
			InsertOperationOutput<CountryDetails> insertOperationOutput = new InsertOperationOutput<CountryDetails>();
			insertOperationOutput.Result = InsertOperationResult.Failed;
			insertOperationOutput.InsertedObject = null;

			int countryId = -1;
			ICountryDataManager countryDataManager = DemoModuleFactory.GetDataManager<ICountryDataManager>();
			bool insertActionSuccess = countryDataManager.Insert(country, out countryId);
			if (insertActionSuccess)
			{
				country.CountryId = countryId;
				insertOperationOutput.Result = InsertOperationResult.Succeeded;
				insertOperationOutput.InsertedObject = CountryDetailMapper(country);
			}
			else
			{
				insertOperationOutput.Result = InsertOperationResult.SameExists;
			}
			return insertOperationOutput;
		}

		public UpdateOperationOutput<CountryDetails> UpdateCountry(Entities.Country country)
		{
			UpdateOperationOutput<CountryDetails> updateOperationOutput = new UpdateOperationOutput<CountryDetails>();
			updateOperationOutput.Result = UpdateOperationResult.Failed;
			updateOperationOutput.UpdatedObject = null;

			ICountryDataManager countryDataManager = DemoModuleFactory.GetDataManager<ICountryDataManager>();
			bool updateActionSuccess = countryDataManager.Update(country);
			if (updateActionSuccess)
			{
				updateOperationOutput.Result = UpdateOperationResult.Succeeded;
				updateOperationOutput.UpdatedObject = CountryDetailMapper(country);
			}
			else
			{
				updateOperationOutput.Result = UpdateOperationResult.Failed;
			}
			return updateOperationOutput;
		}

		#endregion

		#region Private Methods

		private Dictionary<int, Entities.Country> GetCachedCountries()
		{
			return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedCountries", () =>
			{
				ICountryDataManager countryDataManager = DemoModuleFactory.GetDataManager<ICountryDataManager>();
				List<Entities.Country> countries = countryDataManager.GetCountries();
				return countries.ToDictionary(country => country.CountryId, country => country);
			});
		}

		#endregion

		#region Private Classes

		private class CacheManager : Vanrise.Caching.BaseCacheManager
		{
			ICountryDataManager countryDataManager = DemoModuleFactory.GetDataManager<ICountryDataManager>();
			object _updateHandle;

			protected override bool ShouldSetCacheExpired(object parameter)
			{
				return countryDataManager.AreCountryUpdated(ref _updateHandle);
			}
		}

		#endregion

		#region Mappers

		public CountryDetails CountryDetailMapper(Entities.Country country)
		{
			CountryDetails countryDetails = new CountryDetails()
			{
				CountryId = country.CountryId,
				Name = country.Name,
				Population = country.Settings.Population,
				Capital = country.Settings.Capital
			};

			return countryDetails;
		}

		public Entities.CountryInfo CountryInfoMapper(Entities.Country country)
		{
			return new Entities.CountryInfo
			{
				CountryId = country.CountryId,
				Name = country.Name
			};
		}

		#endregion
	}
}