using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "OverriddenConfiguration")]
    [JSONWithTypeAttribute]
    public class OverriddenConfigurationController : BaseAPIController
    {
            [HttpPost]
            [Route("GetFilteredOverriddenConfigurations")]
            public object GetFilteredOverriddenConfigurations(Vanrise.Entities.DataRetrievalInput<OverriddenConfigurationQuery> input)
            {
                OverriddenConfigurationManager manager = new OverriddenConfigurationManager();
                return GetWebResponse(input, manager.GetFilteredOverriddenConfigurations(input));
            }

            [HttpGet]
            [Route("GetOverriddenConfigurationHistoryDetailbyHistoryId")]
            public OverriddenConfiguration GetOverriddenConfigurationHistoryDetailbyHistoryId(int overriddenConfigurationhistoryId)
            {
                OverriddenConfigurationManager manager = new OverriddenConfigurationManager();
                return manager.GetOverriddenConfigurationHistoryDetailbyHistoryId(overriddenConfigurationhistoryId);
            }

            [HttpGet]
            [Route("GetOverriddenConfiguration")]
            public OverriddenConfiguration GetOverriddenConfiguration(Guid overriddenConfigurationId)
            {
                OverriddenConfigurationManager manager = new OverriddenConfigurationManager();
                return manager.GetOverriddenConfiguration(overriddenConfigurationId, true);
            }

            [HttpPost]
            [Route("AddOverriddenConfiguration")]
            public Vanrise.Entities.InsertOperationOutput<OverriddenConfigurationDetail> AddOverriddenConfiguration(OverriddenConfiguration overriddenConfigObject)
            {
                OverriddenConfigurationManager manager = new OverriddenConfigurationManager();
                return manager.AddOverriddenConfiguration(overriddenConfigObject);
            }

            [HttpPost]
            [Route("UpdateOverriddenConfiguration")]
            public Vanrise.Entities.UpdateOperationOutput<OverriddenConfigurationDetail> UpdateOverriddenConfiguration(OverriddenConfiguration overriddenConfigObject)
            {
                OverriddenConfigurationManager manager = new OverriddenConfigurationManager();
                return manager.UpdateOverriddenConfiguration(overriddenConfigObject);
            }
            [HttpGet]
            [Route("GetOverriddenConfigSettingConfigs")]
            public IEnumerable<OverriddenConfigurationConfig> GetOverriddenConfigSettingConfigs()
            {
                OverriddenConfigurationManager manager = new OverriddenConfigurationManager();
                return manager.GetOverriddenConfigSettingConfigs();
            }
           
    }
}