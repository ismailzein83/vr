using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "LOB")]

    public class LOBController : BaseAPIController
    {
        LOBManager lobManager = new LOBManager();

        [HttpGet]
        [Route("GetLOBInfo")]
        public IEnumerable<LOBInfo> GetLOBInfo(string filter = null)
        {
            LOBInfoFilter lobInfoFilter = string.IsNullOrEmpty(filter) ? null : Vanrise.Common.Serializer.Deserialize<LOBInfoFilter>(filter);
            return lobManager.GetLOBInfo(lobInfoFilter);
        }
    }
}