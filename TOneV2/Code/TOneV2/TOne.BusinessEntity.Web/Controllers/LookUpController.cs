using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Web.Controllers
{
    public class LookUpController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public List<Country> GetCountries()
        {
            LookUpManager manager = new LookUpManager();
            return manager.GetCountries();
        }
        [HttpGet]
        public List<City> GetCities()
        {
            LookUpManager manager = new LookUpManager();
            return manager.GetCities();
        }
    }
}