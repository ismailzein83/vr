using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRRestCurrency")]
    public class VRRestCurrencyController : BaseAPIController
    {
        [HttpGet]
        [Route("GetRemoteAllCurrencies")]
        public IEnumerable<Currency> GetRemoteAllCurrencies(Guid connectionId)
        {
            VRRestCurrencyManager manager = new VRRestCurrencyManager();
            return manager.GetRemoteAllCurrencies(connectionId);
        }
        [HttpGet]
        [Route("GetRemoteSystemCurrency")]
        public Currency GetRemoteSystemCurrency(Guid connectionId)
        {
            VRRestCurrencyManager manager = new VRRestCurrencyManager();
            return manager.GetRemoteSystemCurrency(connectionId);
        }
    }
}