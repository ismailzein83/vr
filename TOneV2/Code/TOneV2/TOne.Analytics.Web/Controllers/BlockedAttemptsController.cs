using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.Analytics.Business;

namespace TOne.Analytics.Web.Controllers
{
    public class BlockedAttemptsController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetBlockedAttempts(Vanrise.Entities.DataRetrievalInput<string> input)
        {
            BlockedAttemptsManager manager = new BlockedAttemptsManager();
            return GetWebResponse(input, manager.GetBlockedAttempts(input));

        }
    }
}