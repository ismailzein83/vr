using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{

    [RoutePrefix(Constants.ROUTE_PREFIX + "ZoneServiceConfig")]
    public class WhSBE_ZoneServiceConfigController : BaseAPIController
    {

        [HttpPost]
        [Route("GetFilteredZoneServiceConfigs")]
        public object GetFilteredZoneServiceConfigs(Vanrise.Entities.DataRetrievalInput<ZoneServiceConfigQuery> input)
        {
            ZoneServiceConfigManager manager = new ZoneServiceConfigManager();
            return GetWebResponse(input, manager.GetFilteredZoneServiceConfigs(input));
        }

        [HttpGet]
        [Route("GetAllZoneServiceConfigs")]
        public IEnumerable<ZoneServiceConfig> GetAllZoneServiceConfigs()
        {
            ZoneServiceConfigManager manager = new ZoneServiceConfigManager();
            return manager.GetAllZoneServiceConfigs();
        }
        [HttpGet]
        [Route("GetZoneServiceConfig")]
        public ZoneServiceConfig GetZoneServiceConfig(Int16 serviceFlag)
        {
            ZoneServiceConfigManager manager = new ZoneServiceConfigManager();
            return manager.GetZoneServiceConfig(serviceFlag);
        }

        [HttpPost]
        [Route("AddZoneServiceConfig")]
        public TOne.Entities.InsertOperationOutput<ZoneServiceConfig> AddZoneServiceConfig(ZoneServiceConfig zoneServiceConfig)
        {
            ZoneServiceConfigManager manager = new ZoneServiceConfigManager();
            return manager.AddZoneServiceConfig(zoneServiceConfig);
        }
        [HttpPost]
        [Route("UpdateZoneServiceConfig")]
        public TOne.Entities.UpdateOperationOutput<ZoneServiceConfig> UpdateZoneServiceConfig(ZoneServiceConfig zoneServiceConfig)
        {
            ZoneServiceConfigManager manager = new ZoneServiceConfigManager();
            return manager.UpdateZoneServiceConfig(zoneServiceConfig);
        }

    }
}