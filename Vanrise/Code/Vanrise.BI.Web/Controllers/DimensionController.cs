using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.BI.Business;
using Vanrise.BI.Entities;
using Vanrise.Web.Base;

namespace Vanrise.BI.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Dimension")]
    public class DimensionController:BaseAPIController
    {
        [HttpGet]
        [Route("GetDimensionInfo")]
        public IEnumerable<DimensionInfo> GetDimensionInfo(string entityName)
        {
            DimensionManager manager = new DimensionManager();
            return manager.GetDimensionInfo(entityName);
        }
    }
}