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
    [RoutePrefix(Constants.ROUTE_PREFIX + "VoiceInternationalIdentification")]
    [JSONWithTypeAttribute]
    public class VoiceInternationalIdentificationController : BaseAPIController
    {
        [HttpGet]
        [Route("GetInternationalIdentificationTemplates")]
        public IEnumerable<InternationalIdentificationTemplate> GetInternationalIdentificationTemplates()
        {
            InternationalIdentificationManager manager = new InternationalIdentificationManager();
            return manager.GetInternationalIdentificationTemplates();
        }
    }
}