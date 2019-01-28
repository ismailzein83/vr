using System.Collections.Generic;
using Vanrise.Web.Base;
using System.Web.Http;
using TOne.WhS.Jazz.Business;
using TOne.WhS.Jazz.Entities;
namespace TOne.WhS.Jazz.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SwitchCode")]
    public class SwitchCodeController : BaseAPIController
    {
        SwitchCodeManager _manager = new SwitchCodeManager();

        [HttpGet]
        [Route("GetSwitchCodesInfo")]
        public IEnumerable<SwitchCodeDetail> GetSwitchCodesInfo(string filter=null)
        {
            SwitchCodeInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<SwitchCodeInfoFilter>(filter) : null;
            return _manager.GetSwitchCodesInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetAllSwitchCodes")]
        public IEnumerable<SwitchCode> GetAllSwitchCodes()
        {
            return _manager.GetAllSwitchCodes();
        }
    }
}