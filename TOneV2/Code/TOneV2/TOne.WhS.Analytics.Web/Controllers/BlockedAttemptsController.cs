using System.Web.Http;
using TOne.WhS.Analytics.Business;
using TOne.WhS.Analytics.Entities;
namespace TOne.WhS.Analytics.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "BlockedAttempts")]
    public class WhS_BlockedAttemptsController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetBlockedAttemptsData")]
        public object GetBlockedAttemptsData(Vanrise.Entities.DataRetrievalInput<BlockedAttemptQuery> input)
        {
            BlockedAttemptsManager blockedAttemptsManager = new BlockedAttemptsManager();
            return GetWebResponse(input, blockedAttemptsManager.GetBlockedAttemptData(input), "Blocked Attempts");

        }
    }
}