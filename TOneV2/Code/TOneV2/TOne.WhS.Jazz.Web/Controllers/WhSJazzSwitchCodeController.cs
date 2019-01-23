using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vanrise.Web.Base;
using System.Web.Http;
using Vanrise.Entities;
using TOne.WhS.Jazz.Web;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using TOne.WhS.Jazz.Business;
using TOne.WhS.Jazz.Entities;
using Vanrise.Common;
namespace TOne.WhS.Jazz.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "WhSJazzSwitchCode")]
    public class WhSJazzSwitchCodeController : BaseAPIController
    {
        WhSJazzSwitchCodeManager _manager = new WhSJazzSwitchCodeManager();

        [HttpGet]
        [Route("GetSwitchCodesInfo")]
        public IEnumerable<WhSJazzSwitchCodeDetail> GetSwitchCodesInfo(string filter=null)
        {
            WhSJazzSwitchCodeInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<WhSJazzSwitchCodeInfoFilter>(filter) : null;
            return _manager.GetSwitchCodesInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetAllSwitchCodes")]
        public IEnumerable<WhSJazzSwitchCode> GetAllSwitchCodes()
        {
            return _manager.GetAllSwitchCodes();
        }
    }
}