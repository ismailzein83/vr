using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Demo.Module.Entities;
using Demo.Module.Business;
using Vanrise.Entities;


namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Service")]
    [JSONWithTypeAttribute]
    public class ServiceController : BaseAPIController
    {
        ServiceManager serviceManager = new ServiceManager();

        [HttpGet]
        [Route("GetServices")]
        public List<Service> GetServices()
        {
            return serviceManager.GetServices();

        }
       
    }
}
