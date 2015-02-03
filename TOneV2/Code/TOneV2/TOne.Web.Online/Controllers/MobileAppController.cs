using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.Web.Online.Models;

namespace TOne.Web.Online.Controllers
{
    public class MobileAppController : ApiController
    {
        [HttpGet]
        public MobileAppConfig GetConfig()
        {
            return new MobileAppConfig
            {
                UserDisplayName = SecurityToken.Current.UserDisplayName
            };
        }
    }
}