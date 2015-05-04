using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.LCR.Entities.Routing;

namespace TOne.Web.Online.Controllers
{
    public class LCRController : ApiController
    {
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
        public List<RouteInfo> GetRoutes(string customerId, string target, TargetType targetType, int routesCount, int lcrCount)
        {
            return new List<RouteInfo>();
        }
    }
}