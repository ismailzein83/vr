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
    [RoutePrefix(Constants.ROUTE_PREFIX + "WhSJazzRegionCode")]
    public class WhSJazzRegionCodeController:BaseAPIController
    {
        WhSJazzRegionCodeManager _manager = new WhSJazzRegionCodeManager();

        [HttpGet]
        [Route("GetRegionCodesInfo")]
        public IEnumerable<WhSJazzRegionCodeDetail> GetRegionCodesInfo(string filter=null)
        {
            WhSJazzRegionCodeInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<WhSJazzRegionCodeInfoFilter>(filter) : null;
            return _manager.GetRegionCodesInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetAllRegionCodes")]
        public IEnumerable<WhSJazzRegionCode> GetAllNodesStates()
        {
            return _manager.GetAllRegionCodes();
        }
    }
}