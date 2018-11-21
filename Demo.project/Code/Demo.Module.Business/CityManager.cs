using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Demo.Module.Business
{
	public class CityManager
	{
		CountryManager _countyManager = new CountryManager();

		#region Public Methods

		public IDataRetrievalResult<CityDetails> GetFilteredCities(DataRetrievalInput<Entities.CityQuery> input)
		{
			Dictionary<int, Entities.City> allCities = GetCachedCities();

			Func<Entities.City, bool> filterExpression = (city) =>
			{
				if (input.Query != null)
				{
					if (input.Query.Name != null && !city.Name.ToLower().Contains(input.Query.Name.ToLower()))
						return false;

					if (input.Query.CountryIds != null && !input.Query.CountryIds.Contains(city.CountryId))
						return false;

					return true;
				}
				return true;
			};

			return DataRetrievalManager.Instance.ProcessResult(input, allCities.ToBigResult(input, filterExpression, CityDetailMapper));
		}

		public Entities.City GetCityById(int cityId)
		{
			Dictionary<int, Entities.City> allCities = GetCachedCities();
			return allCities.GetRecord(cityId);
		}

		public IEnumerable<DistrictSettingsConfig> GetDistrictSettingsConfigs()
		{
			var extensionConfigurationManager = new ExtensionConfigurationManager();
			return extensionConfigurationManager.GetExtensionConfigurations<DistrictSettingsConfig>(DistrictSettingsConfig.EXTENSION_TYPE);
		}

		public IEnumerable<CityTypeConfig> GetCityTypesConfigs()
		{
			var extensionConfigurationManager = new ExtensionConfigurationManager();
			return extensionConfigurationManager.GetExtensionConfigurations<CityTypeConfig>(CityTypeConfig.EXTENSION_TYPE);
		}

		public IEnumerable<FactoryTypeConfig> GetFactoryTypesConfigs()
		{
			var extensionConfigurationManager = new ExtensionConfigurationManager();
			return extensionConfigurationManager.GetExtensionConfigurations<FactoryTypeConfig>(FactoryTypeConfig.EXTENSION_TYPE);

		}

		public InsertOperationOutput<CityDetails> AddCity(Entities.City city)
		{
			InsertOperationOutput<CityDetails> insertOperationOutput = new InsertOperationOutput<Entities.CityDetails>();
			insertOperationOutput.Result = InsertOperationResult.Failed;
			insertOperationOutput.InsertedObject = null;

			int cityId = -1;
			ICityDataManager cityDataManager = DemoModuleFactory.GetDataManager<ICityDataManager>();
			bool insertActionSuccess = cityDataManager.Insert(city, out cityId);
			if (insertActionSuccess)
			{
				city.CityId = cityId;
				insertOperationOutput.Result = InsertOperationResult.Succeeded;
				insertOperationOutput.InsertedObject = CityDetailMapper(city);
			}
			else
			{
				insertOperationOutput.Result = InsertOperationResult.SameExists;
			}
			return insertOperationOutput;
		}

		public UpdateOperationOutput<CityDetails> UpdateCity(Entities.City city)
		{
			UpdateOperationOutput<CityDetails> updateOperationOutput = new UpdateOperationOutput<CityDetails>();
			updateOperationOutput.Result = UpdateOperationResult.Failed;
			updateOperationOutput.UpdatedObject = null;

			ICityDataManager cityDataManager = DemoModuleFactory.GetDataManager<ICityDataManager>();
			bool updateActionsuccess = cityDataManager.Update(city);
			if (updateActionsuccess)
			{
				updateOperationOutput.Result = UpdateOperationResult.Succeeded;
				updateOperationOutput.UpdatedObject = CityDetailMapper(city);
			}
			else
			{
				updateOperationOutput.Result = UpdateOperationResult.SameExists;
			}
			return updateOperationOutput;
		}

		#endregion

		#region Private Methods

		private Dictionary<int, Entities.City> GetCachedCities()
		{
			return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedCities", () =>
			{
				ICityDataManager cityDataManager = DemoModuleFactory.GetDataManager<ICityDataManager>();
				List<Entities.City> cities = cityDataManager.GetCities();
				return cities.ToDictionary(city => city.CityId, city => city);
			});
		}

		#endregion

		#region Private Classes

		private class CacheManager : Vanrise.Caching.BaseCacheManager
		{
			ICityDataManager cityDataManager = DemoModuleFactory.GetDataManager<ICityDataManager>();
			object _updateHandle;

			protected override bool ShouldSetCacheExpired(object parameter)
			{
				return cityDataManager.AreCityUpdated(ref _updateHandle);
			}
		}

		#endregion

		#region Mappers

		public CityDetails CityDetailMapper(Entities.City city)
		{
			city.ThrowIfNull("city", city.CityId);
			city.Settings.ThrowIfNull("city.Settings", city.CityId);
			city.Settings.CityType.ThrowIfNull("city.Settings.CityType", city.CityId);
			city.Settings.Districts.ThrowIfNull("city.Settings.Districts", city.CityId);

			IEnumerable<string> districtNames = city.Settings.Districts.Select(itm => itm.Name);

			CityDetails cityDetails = new CityDetails()
			{
				CityId = city.CityId,
				Name = city.Name,
				CountryName = _countyManager.GetCountryName(city.CountryId),
				CitySettingsDescription = city.Settings.CityType.GetDescription(),
				DistrictsNames = string.Join(", ", districtNames)
			};

			return cityDetails;
		}

		#endregion
	}
}