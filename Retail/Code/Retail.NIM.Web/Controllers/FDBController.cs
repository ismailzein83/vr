using Retail.NIM.Business;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.NIM.Web.Controller
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "FDB")]
    [JSONWithTypeAttribute]
    public class FDBController : BaseAPIController
    {
        FDBManager fdbManager = new FDBManager();

        [HttpPost]
        [Route("GetFDBReservationInfo")]
        public GetFDBReservationInfoOutput GetFDBReservationInfo(GetFDBReservationInfoInput input)
        {
            return fdbManager.GetFDBReservationInfo(input);
        }
    }
}