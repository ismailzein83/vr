﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Web.Base;

namespace Vanrise.Analytic.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AnalyticItemConfig")]
    public class AnalyticItemConfigController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetDimensionsInfo")]
        public IEnumerable<AnalyticDimensionConfigInfo> GetDimensionsInfo(string filter)
        {
            AnalyticItemConfigManager manager = new AnalyticItemConfigManager();
            AnalyticDimensionConfigInfoFilter serializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<AnalyticDimensionConfigInfoFilter>(filter) : null;
            return manager.GetDimensionsInfo(serializedFilter);
        }

        [HttpGet]
        [Route("GetMeasuresInfo")]
        public IEnumerable<AnalyticMeasureConfigInfo> GetMeasuresInfo(string filter)
        {
            AnalyticItemConfigManager manager = new AnalyticItemConfigManager();
            AnalyticMeasureConfigInfoFilter serializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<AnalyticMeasureConfigInfoFilter>(filter) : null;
            return manager.GetMeasuresInfo(serializedFilter);
        }
        [HttpGet]
        [Route("GetJoinsInfo")]
        public IEnumerable<AnalyticJoinConfigInfo> GetJoinsInfo(string filter)
        {
            AnalyticItemConfigManager manager = new AnalyticItemConfigManager();
            AnalyticJoinConfigInfoFilter serializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<AnalyticJoinConfigInfoFilter>(filter) : null;
            return manager.GetJoinsInfo(serializedFilter);
        }
        [HttpPost]
        [Route("GetFilteredAnalyticItemConfigs")]
        public object GetFilteredAnalyticItemConfigs(Vanrise.Entities.DataRetrievalInput<AnalyticItemConfigQuery> input)
        {
            AnalyticItemConfigManager manager = new AnalyticItemConfigManager();
            return GetWebResponse(input, manager.GetFilteredAnalyticItemConfigs(input));
        }
        [HttpGet]
        [Route("GetAnalyticItemConfigsById")]
        public Object GetAnalyticItemConfigsById(int tableId, AnalyticItemType itemType, int analyticItemConfigId)
        {
            AnalyticItemConfigManager manager = new AnalyticItemConfigManager();
            return manager.GetAnalyticItemConfigsById(tableId, itemType, analyticItemConfigId);
        }

        [HttpPost]
        [Route("AddAnalyticItemConfig")]
        public Object AddAnalyticItemConfig(AnalyticItemConfigInput analyticItemConfig)
        {
            AnalyticItemConfigManager manager = new AnalyticItemConfigManager();
            switch (analyticItemConfig.ItemType)
            {
                case AnalyticItemType.Dimension:
                    var dimensionAnalyticItemConfig = Serializer.Deserialize<AnalyticItemConfig<AnalyticDimensionConfig>>(analyticItemConfig.AnalyticItemConfig);
                    return manager.AddAnalyticItemConfig(dimensionAnalyticItemConfig);
                case AnalyticItemType.Measure:
                    var measureAnalyticItemConfigObj = Serializer.Deserialize<AnalyticItemConfig<AnalyticMeasureConfig>>(analyticItemConfig.AnalyticItemConfig);
                    return manager.AddAnalyticItemConfig(measureAnalyticItemConfigObj);
                case AnalyticItemType.Join:
                    var joinAnalyticItemConfigObj = Serializer.Deserialize<AnalyticItemConfig<AnalyticJoinConfig>>(analyticItemConfig.AnalyticItemConfig);
                    return manager.AddAnalyticItemConfig(joinAnalyticItemConfigObj);
            }
            return null;
        }
        [HttpPost]
        [Route("UpdateAnalyticItemConfig")]
        public Object UpdateAnalyticItemConfig(AnalyticItemConfigInput analyticItemConfig)
        {
            AnalyticItemConfigManager manager = new AnalyticItemConfigManager();
            switch (analyticItemConfig.ItemType)
            {
                case AnalyticItemType.Dimension:
                    var dimensionAnalyticItemConfig = Serializer.Deserialize<AnalyticItemConfig<AnalyticDimensionConfig>>(analyticItemConfig.AnalyticItemConfig);
                    return manager.UpdateAnalyticItemConfig(dimensionAnalyticItemConfig);
                case AnalyticItemType.Measure:
                    var measureAnalyticItemConfigObj = Serializer.Deserialize<AnalyticItemConfig<AnalyticMeasureConfig>>(analyticItemConfig.AnalyticItemConfig);
                    return manager.UpdateAnalyticItemConfig(measureAnalyticItemConfigObj);
                case AnalyticItemType.Join:
                    var joinAnalyticItemConfigObj = Serializer.Deserialize<AnalyticItemConfig<AnalyticJoinConfig>>(analyticItemConfig.AnalyticItemConfig);
                    return manager.UpdateAnalyticItemConfig(joinAnalyticItemConfigObj);
            }
            return null;
        }
    }
    
}