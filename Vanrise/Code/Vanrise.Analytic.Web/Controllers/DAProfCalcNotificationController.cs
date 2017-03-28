using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Analytic.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "DAProfCalcNotification")]
    [JSONWithTypeAttribute]
    public class DAProfCalcNotificationController : BaseAPIController
    {
        DAProfCalcNotificationManager _manager = new DAProfCalcNotificationManager();

        [HttpGet]
        [Route("GetDAProfCalcNotificationTypeId")]
        public Guid GetDAProfCalcNotificationTypeId(Guid alertRuleTypeId, Guid dataAnalysisItemDefinitionId)
        {
            return _manager.GetDAProfCalcNotificationTypeId(alertRuleTypeId, dataAnalysisItemDefinitionId);
        }
    }
}