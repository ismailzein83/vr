using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "City")]
    public class VRCommon_CityController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredCities")]
        public object GetFilteredCities(Vanrise.Entities.DataRetrievalInput<CityQuery> input)
        {
            CityManager manager = new CityManager();
            return GetWebResponse(input, manager.GetFilteredCities(input), "Cities");
        }

        [HttpGet]
        [Route("GetCityHistoryDetailbyHistoryId")]
        public City GetCityHistoryDetailbyHistoryId(int cityHistoryId)
        {
            CityManager manager = new CityManager();
            return manager.GetCityHistoryDetailbyHistoryId(cityHistoryId);
        }

        [HttpGet]
        [Route("GetCitiesInfo")]
        public IEnumerable<CityInfo> GetCitiesInfo(int countryId)
        {
            CityManager manager = new CityManager();
            return manager.GetCitiesInfo(countryId);
        }

        [HttpPost]
        [Route("GetDistinctCountryIdsByCityIds")]
        public IEnumerable<int> GetDistinctCountryIdsByCityIds(IEnumerable<int> cityIds)
        {
           CityManager manager = new CityManager();
           return manager.GetDistinctCountryIdsByCityIds(cityIds);
        }

        [HttpGet]
        [Route("GetCity")]
        public City GetCity(int cityId)
        {
            CityManager manager = new CityManager();
            return manager.GetCity(cityId,true);
        }

        [HttpPost]
        [Route("AddCity")]
        public Vanrise.Entities.InsertOperationOutput<CityDetail> AddCity(City city)
        {
            CityManager manager = new CityManager();
            return manager.AddCity(city);
        }
        
        [HttpPost]
        [Route("UpdateCity")]
        public Vanrise.Entities.UpdateOperationOutput<CityDetail> UpdateCity(City city)
        {
            CityManager manager = new CityManager();
            return manager.UpdateCity(city);
        }
    }
}