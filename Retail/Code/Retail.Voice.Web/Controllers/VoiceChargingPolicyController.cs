using Retail.Voice.Business;
using Retail.Voice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.Voice.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VoiceChargingPolicy")]
    [JSONWithTypeAttribute]
    public class VoiceChargingPolicyController : BaseAPIController
    {
        [HttpGet]
        [Route("GetVoiceChargingPolicyEvaluatorTemplateConfigs")]
        public IEnumerable<VoiceChargingPolicyEvaluatorConfig> GetVoiceChargingPolicyEvaluatorTemplateConfigs()
        {
            VoiceChargingPolicyEvaluatorManager manager = new VoiceChargingPolicyEvaluatorManager();
            return manager.GetVoiceChargingPolicyEvaluatorTemplateConfigs();
        }
    }
}