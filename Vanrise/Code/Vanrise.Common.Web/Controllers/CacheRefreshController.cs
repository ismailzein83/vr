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
    [RoutePrefix(Constants.ROUTE_PREFIX + "CacheRefreshHandle")]
    public class VRCommon_CacheRefreshHandleController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredCacheRefreshHandles")]
        public object GetFilteredCacheRefreshHandles(Vanrise.Entities.DataRetrievalInput<CacheRefreshHandleQuery> input)
        {
            CacheRefreshManager manager = new CacheRefreshManager();
            return GetWebResponse(input, manager.GetFilteredCacheRefreshHandles(input), "Cache Refresh Handles");
        }


        [HttpGet]
        [Route("SetCacheExpired")]
        public void SetCacheExpired(string typeName)
        {
            CacheRefreshManager manager = new CacheRefreshManager();
            manager.TriggerCacheExpiration(typeName);
        }
    }
}