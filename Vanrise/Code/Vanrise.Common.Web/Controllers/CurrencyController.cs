using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
namespace Vanrise.Web.Controllers
{
   [RoutePrefix(Constants.ROUTE_PREFIX + "Currency")]
    public class VRCommon_CurrencyController  : BaseAPIController
    {
       [HttpPost]
       [Route("GetFilteredCurrencies")]
       public object GetFilteredCurrencies(Vanrise.Entities.DataRetrievalInput<CurrencyQuery> input)
       {
           CurrencyManager manager = new CurrencyManager();
           return GetWebResponse(input, manager.GetFilteredCurrencies(input));
       }
       
    }
}
