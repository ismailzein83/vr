using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common;

namespace TOne.WhS.Routing.Business
{
    public class TrafficStatisticQCDefinitionManager
    {
        public TrafficStatisticQCDefinitionData GetTrafficStatisticQCDefinitionData(Guid qualityConfigurationDefinitionId)
        {
            QualityConfigurationDefinitionManager qualityConfigurationDefinitionManager = new QualityConfigurationDefinitionManager();
            QualityConfigurationDefinitionExtendedSettings extendedSettings = qualityConfigurationDefinitionManager.GetQualityConfigurationDefinitionExtendedSettings(qualityConfigurationDefinitionId);
            var trafficStatisticQCDefinitionSettings = extendedSettings.CastWithValidate<TrafficStatisticQCDefinitionSettings>("qualityConfigurationDefinitionExtendedSettings", qualityConfigurationDefinitionId);
            List<string> includedMeasures = trafficStatisticQCDefinitionSettings.IncludedMeasures;
            if (includedMeasures == null || includedMeasures.Count == 0)
                throw new Exception("trafficStatisticQCDefinitionSettings.IncludedMeasures should contains at least one measure.");

            Dictionary<string, AnalyticMeasure> analyticItemConfigs = new AnalyticItemConfigManager().GetMeasures(trafficStatisticQCDefinitionSettings.AnalyticTableId);
            List<AnalyticMeasureInfo> analyticMeasureInfos = new List<AnalyticMeasureInfo>();

            if (analyticItemConfigs != null)
            {
                AnalyticMeasure analyticMeasure;
                foreach (var measureName in includedMeasures)
                {
                    if (analyticItemConfigs.TryGetValue(measureName, out analyticMeasure))
                        analyticMeasureInfos.Add(new AnalyticMeasureInfo { Name = measureName, Title = analyticMeasure.Title });
                }
            }

            return new TrafficStatisticQCDefinitionData()
            {
                AnalyticMeasureInfos = analyticMeasureInfos,
                TimeSettingsDirective = trafficStatisticQCDefinitionSettings.TimeSettingsDirective
            };
        }
    }

    public class TrafficStatisticQCDefinitionData
    {
        public List<AnalyticMeasureInfo> AnalyticMeasureInfos { get; set; }

        public string TimeSettingsDirective { get; set; }
    }
}