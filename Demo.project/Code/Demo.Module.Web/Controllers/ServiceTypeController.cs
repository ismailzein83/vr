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
    [RoutePrefix(Constants.ROUTE_PREFIX + "ServiceType")]
    public class Demo_ServiceTypeController : BaseAPIController
    {
        [HttpGet]
        [Route("GetServiceTypesInfo")]
        public IEnumerable<ServiceType> GetServiceTypesInfo()
        {
            ServiceTypeManager manager = new ServiceTypeManager();
            return manager.GetServiceTypesInfo();
        }


    }

}