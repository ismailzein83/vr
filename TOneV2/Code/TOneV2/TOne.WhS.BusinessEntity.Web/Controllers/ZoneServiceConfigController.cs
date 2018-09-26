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
            return GetWebResponse(input, manager.GetFilteredZoneServiceConfigs(input), "Zone Service Configurations");
        }

        [HttpGet]
        [Route("GetAllZoneServiceConfigs")]
        public IEnumerable<ZoneServiceConfigInfo> GetAllZoneServiceConfigs(string serializedFilter)
        {
            ZoneServiceConfigFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<ZoneServiceConfigFilter>(serializedFilter) : null;
            ZoneServiceConfigManager manager = new ZoneServiceConfigManager();
            return manager.GetAllZoneServiceConfigs(filter);
        }

        [HttpGet]
        [Route("GetAllZoneServices")]
        public IEnumerable<ZoneServiceConfig> GetAllZoneServices()
        {
            ZoneServiceConfigManager manager = new ZoneServiceConfigManager();
            return manager.GetAllZoneServices();
        }
        [HttpGet]
        [Route("GetZoneServiceConfig")]
        public ZoneServiceConfig GetZoneServiceConfig(int zoneServiceConfigId)
        {
            ZoneServiceConfigManager manager = new ZoneServiceConfigManager();
            return manager.GetZoneServiceConfig(zoneServiceConfigId,true);
        }

        [HttpPost]
        [Route("AddZoneServiceConfig")]
        public InsertOperationOutput<ZoneServiceConfigDetail> AddZoneServiceConfig(ZoneServiceConfig zoneServiceConfig)
        {
            ZoneServiceConfigManager manager = new ZoneServiceConfigManager();
            return manager.AddZoneServiceConfig(zoneServiceConfig);
        }
        [HttpPost]
        [Route("UpdateZoneServiceConfig")]
        public UpdateOperationOutput<ZoneServiceConfigDetail> UpdateZoneServiceConfig(ZoneServiceConfig zoneServiceConfig)
        {
            ZoneServiceConfigManager manager = new ZoneServiceConfigManager();
            return manager.UpdateZoneServiceConfig(zoneServiceConfig);
        }

    }
}