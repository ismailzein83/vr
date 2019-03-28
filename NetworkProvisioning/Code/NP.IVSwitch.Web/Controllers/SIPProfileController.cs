using NP.IVSwitch.Business;
using NP.IVSwitch.Entities;
using NP.IVSwitch.Entities.SIPProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace NP.IVSwitch.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "SIPProfile")]
    [JSONWithTypeAttribute]
    public class SIPProfileController : BaseAPIController
    {

        SIPProfileManager _manager = new SIPProfileManager();

        [HttpGet]
        [Route("GetSIPProfilesInfo")]
        public IEnumerable<SIPProfileInfo> GetSIPProfilesInfo(string filter = null)
        {
            return _manager.GetSIPProfilesInfo();
        }
    }
}