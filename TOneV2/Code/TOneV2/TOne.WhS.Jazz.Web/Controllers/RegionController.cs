using System.Collections.Generic;
using Vanrise.Web.Base;
using System.Web.Http;
using TOne.WhS.Jazz.Business;
using TOne.WhS.Jazz.Entities;
namespace TOne.WhS.Jazz.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Region")]
    public class RegionController:BaseAPIController
    {
        RegionManager _manager = new RegionManager();

        [HttpGet]
        [Route("GetRegionsInfo")]
        public IEnumerable<Entities.RegionDetail> GetRegionsInfo(string filter=null)
        {
            RegionInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<RegionInfoFilter>(filter) : null;
            return _manager.GetRegionsInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetAllRegions")]
        public IEnumerable<Entities.Region> GetAllNodesStates()
        {
            return _manager.GetAllRegions();
        }
    }
}