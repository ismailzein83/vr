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
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "VoiceAccountIdentification")]
    public class VoiceAccountIdentificationController : BaseAPIController
    {
        [HttpGet]
        [Route("GetAccountIdentificationTemplates")]
        public IEnumerable<AccountIdentificationTemplate> GetAccountIdentificationTemplates()
        {
            AccountIdentificationManager manager = new AccountIdentificationManager();
            return manager.GetAccountIdentificationTemplates();
        }
    }
}