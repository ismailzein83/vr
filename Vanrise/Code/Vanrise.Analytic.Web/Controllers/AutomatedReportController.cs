using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Analytic.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AutomatedReport")]

    public class AutomatedReportController : BaseAPIController
    {
        AutomatedReportManager _manager = new AutomatedReportManager();

        [HttpGet]
        [Route("GetAutomatedReportSettings")]
        public VRAutomatedReportSettings GetAutomatedReportSettings()
        {
            return _manager.GetAutomatedReportSettings();
        }

    }
}