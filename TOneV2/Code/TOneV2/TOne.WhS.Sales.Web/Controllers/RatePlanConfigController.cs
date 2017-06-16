using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace TOne.WhS.Sales.Web.Controllers
{
    [Vanrise.Web.Base.JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "RatePlanConfig")]
    public class RatePlanConfigController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetGeneralSettingsLongPrecisionValue")]
        public int GetGeneralSettingsLongPrecisionValue()
        {
            return new Vanrise.Common.Business.GeneralSettingsManager().GetLongPrecisionValue();
        }
    }
}