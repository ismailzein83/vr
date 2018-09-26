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
    [RoutePrefix(Constants.ROUTE_PREFIX + "TimeZone")]
    [JSONWithTypeAttribute]
    public class VRCommon_TimeZoneController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredVRTimeZones")]
        public object GetFilteredVRTimeZones(Vanrise.Entities.DataRetrievalInput<VRTimeZoneQuery> input)
        {
            VRTimeZoneManager manager = new VRTimeZoneManager();
            return GetWebResponse(input, manager.GetFilteredVRTimeZones(input), "Time Zones");
        }

        [HttpGet]
        [Route("GetVRTimeZonesInfo")]
        public IEnumerable<VRTimeZoneInfo> GetVRTimeZonesInfo()
        {
            VRTimeZoneManager manager = new VRTimeZoneManager();
            return manager.GetVRTimeZonesInfo();
        }
        [HttpGet]
        [Route("GetVRTimeZone")]
        public VRTimeZone GetVRTimeZone(int timeZoneId)
        {
            VRTimeZoneManager manager = new VRTimeZoneManager();
            return manager.GetVRTimeZone(timeZoneId,true);
        }

        [HttpPost]
        [Route("AddVRTimeZone")]
        public Vanrise.Entities.InsertOperationOutput<VRTimeZoneDetail> AddVRTimeZone(VRTimeZone vrTimeZone)
        {
            VRTimeZoneManager manager = new VRTimeZoneManager();
            return manager.AddVRTimeZone(vrTimeZone);
        }
        
        [HttpPost]
        [Route("UpdateVRTimeZone")]
        public Vanrise.Entities.UpdateOperationOutput<VRTimeZoneDetail> UpdateVRTimeZone(VRTimeZone vrTimeZone)
        {
            VRTimeZoneManager manager = new VRTimeZoneManager();
            return manager.UpdateVRTimeZone(vrTimeZone);
        }
    }
}