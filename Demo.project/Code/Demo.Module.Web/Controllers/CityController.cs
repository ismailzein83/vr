using Demo.Module.Business;
using Demo.Module.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Demo.Module.Web.Controllers
{
	[RoutePrefix(Constants.ROUTE_PREFIX + "Demo_City")]
	[JSONWithTypeAttribute]
	public class CityController : BaseAPIController
	{
		CityManager cityManager = new CityManager();

		[HttpPost]
		[Route("GetFilteredCities")]
		public object GetFilteredCities(DataRetrievalInput<Entities.CityQuery> input)
		{
			return GetWebResponse(input, cityManager.GetFilteredCities(input));
		}

		[HttpGet]	
		[Route("GetCityById")]
		public Entities.City GetCityById(int cityId)
		{
			return cityManager.GetCityById(cityId);	
		}

		[HttpPost]
		[Route("UpdateCity")]
		public UpdateOperationOutput<CityDetails> UpdateCity(Demo.Module.Entities.City city)
		{
			return cityManager.UpdateCity(city);
		}

		[HttpPost]
		[Route("AddCity")]
		public InsertOperationOutput<CityDetails> AddCity(Demo.Module.Entities.City city)
		{
			return cityManager.AddCity(city);
		}

		[HttpGet]
		[Route("GetCityTypesConfigs")]
		public IEnumerable<CityTypeConfig> GetCityTypesConfigs()
		{
			return cityManager.GetCityTypesConfigs();
		}

		[HttpGet]
		[Route("GetDistrictSettingsConfigs")]
		public IEnumerable<DistrictSettingsConfig> GetDistrictSettingsConfigs()
		{
			return cityManager.GetDistrictSettingsConfigs();
		}

		[HttpGet]
		[Route("GetFactoryTypesConfigs")]
		public IEnumerable<FactoryTypeConfig> GetFactoryTypesConfigs()
		{
			return cityManager.GetFactoryTypesConfigs();
		}

	}
}