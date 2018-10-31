using Demo.Module.Business;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "DemoCurrency")]
    [JSONWithTypeAttribute]
    public class DemoCurrencyController : BaseAPIController
    {
        DemoCurrencyManager demoCurrencyManager = new DemoCurrencyManager();
       

        [HttpGet]
        [Route("GetDemoCurrencyById")]
        public DemoCurrency GetDemoCurrencyById(int demoCurrencyId)
        {
            return demoCurrencyManager.GetDemoCurrencyById(demoCurrencyId);
        }

        [HttpGet]
        [Route("GetDemoCurrenciesInfo")]
        public IEnumerable<DemoCurrencyInfo> GetCurrenciesInfo(string filter = null)
        {
            DemoCurrencyInfoFilter DemoCurrencyInfoFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<DemoCurrencyInfoFilter>(filter) : null;
            return demoCurrencyManager.GetDemoCurrenciesInfo(DemoCurrencyInfoFilter);
        }
        

    }
}