using CP.WhS.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Analytics.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace CP.WhS.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "BlockedAttempts")]
    [JSONWithTypeAttribute]
    public class BlockedAttemptsController : BaseAPIController
    {
        BlockedAttemptsManager _blockedAttemptsManager = new BlockedAttemptsManager();
        [HttpPost]
        [Route("GetFilteredBlockedAttempts")]
        public object GetFilteredBlockedAttempts(DataRetrievalInput<BlockedAttemptQuery> input)
        {
            return GetWebResponse(input, _blockedAttemptsManager.GetFilteredBlockedAttempts(input));
        }
    }
}