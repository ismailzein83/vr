using PartnerPortal.CustomerAccess.Business;
using PartnerPortal.CustomerAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace PartnerPortal.CustomerAccess.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Analytic")]
    [JSONWithTypeAttribute]
    public class CPAnalyticController:BaseAPIController
    {
        [HttpPost]
        [Route("GetAnalyticTileInfo")]
        public AnalyticTileInfo GetAnalyticTileInfo(AnalyticDefinitionSettings analyticDefinitionSettings)
        {
            AnalyticManager manager = new AnalyticManager();
            return manager.GetAnalyticTileInfo(analyticDefinitionSettings);
        }
    }
}