using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
using Vanrise.Common.Business;
namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRMail")]
    [JSONWithTypeAttribute]
    public class VRMailController : BaseAPIController
    {
        [HttpPost]
        [Route("SendTestEmail")]
        public void SendTestEmail(EmailSettingDetail setting)
        {
            VRMailManager vrMailManager = new VRMailManager();
            vrMailManager.SendTestMail(setting.EmailSettingData, setting.ToEmail, setting.Subject, setting.Body);
        }
    }
}
