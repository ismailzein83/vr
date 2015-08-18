using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.LCR.Business;
using TOne.LCR.Entities.Routing;

namespace TOne.Web.Online.Controllers
{
    public class LCRController : ApiController
    {
        private readonly OldRouteManager _routeManager;
        public LCRController()
        {
            _routeManager = new OldRouteManager();
        }
        /// <summary>
        /// Get List of Routes 
        /// </summary>
        /// <param name="customerId">Get routes for a specified customer. Required</param>
        /// <param name="target">Target Filter. Examples: 961% for code and its sub. afgh% for Zone</param>
        /// <param name="targetType">Enumeration. Code or Zone</param>
        /// <param name="routesCount">Maximun returned routes</param>
        /// <param name="lcrCount">Maximum count of Suppliers in LCR.</param>
        /// <returns></returns>
        [HttpGet]
        public List<RouteInfo> GetRoutes(bool showBlocks, bool? isBlock, int topValue, int from, int to, string customerId = null, string supplierId = null, string code = null, string zone = null)
        {
            return _routeManager.GetRoutes(showBlocks, isBlock, topValue, from, to, customerId, supplierId, code, zone);
        }
    }
}