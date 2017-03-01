using Demo.Module.Business;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "City")]
    public class Demo_Module_CityController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredCities")]
        public object GetFilteredCities(Vanrise.Entities.DataRetrievalInput<CityQ> input)
        {
            CityManager manager = new CityManager ();
            return GetWebResponse(input, manager.GetFilteredCities(input));
           
        }

        [HttpPost]
        [Route("AddCity")]
        public Vanrise.Entities.InsertOperationOutput<CityDetails> AddCity(City city)
        {
            CityManager manager = new CityManager();
            return manager.AddCity(city);
        }

        [HttpGet]
        [Route("GetCity")]
        public City GetCity(int Id)
        {
            CityManager manager = new CityManager();
            return manager.GetCity(Id);
        }
        [HttpPost]
        [Route("UpdateCity")]
        public Vanrise.Entities.UpdateOperationOutput<CityDetails> UpdateCity(City city)
        {
            CityManager manager = new CityManager();
            return manager.UpdateCity(city);
        }
    
    }
}