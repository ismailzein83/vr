using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Demo.Module.Business;
using Demo.Module.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace  Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "UnitType")]
    public class Demo_UnitTypeController : BaseAPIController
    {
        [HttpGet]
        [Route("GetUnitTypesInfo")]
        public IEnumerable<UnitType> GetUnitTypesInfo()
        {
            UnitTypeManager manager = new UnitTypeManager();
            return manager.GetUnitTypesInfo();
        }


    }

}