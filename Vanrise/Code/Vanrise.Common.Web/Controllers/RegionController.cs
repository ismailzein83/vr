using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Region")]
    public class VRCommon_RegionController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredRegions")]
        public object GetFilteredRegions(Vanrise.Entities.DataRetrievalInput<RegionQuery> input)
        {
            RegionManager manager = new RegionManager();
            return GetWebResponse(input, manager.GetFilteredRegions(input), "Regions");
        }

        [HttpGet]
        [Route("GetRegionHistoryDetailbyHistoryId")]
        public Region GetRegionHistoryDetailbyHistoryId(int RegionHistoryId)
        {
            RegionManager manager = new RegionManager();
            return manager.GetRegionHistoryDetailbyHistoryId(RegionHistoryId);
        }

        [HttpGet]
        [Route("GetRegionsInfo")]
        public IEnumerable<RegionInfo> GetRegionsInfo(string filter = null)
		{
            RegionInfoFilter countryFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<RegionInfoFilter>(filter) : null;
            RegionManager manager = new RegionManager();
            return manager.GetRegionsInfo(countryFilter);
        }
        [HttpGet]
        [Route("GetRegion")]
        public Region GetRegion(int RegionId)
        {
            RegionManager manager = new RegionManager();
            return manager.GetRegion(RegionId,true);
        }

        [HttpPost]
        [Route("AddRegion")]
        public Vanrise.Entities.InsertOperationOutput<RegionDetail> AddRegion(Region Region)
        {
            RegionManager manager = new RegionManager();
            return manager.AddRegion(Region);
        }
        
        [HttpPost]
        [Route("UpdateRegion")]
        public Vanrise.Entities.UpdateOperationOutput<RegionDetail> UpdateRegion(Region Region)
        {
            RegionManager manager = new RegionManager();
            return manager.UpdateRegion(Region);
        }
    }
}