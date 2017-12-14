using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using System.Linq;

namespace TOne.WhS.Routing.Business
{
    public class QualityConfigurationManager
    {
        #region public methods

        public Dictionary<Guid, RouteRuleQualityConfiguration> GetRouteRuleQualityConfigurations()
        {
            ConfigManager configManager = new ConfigManager();
            List<RouteRuleQualityConfiguration> routeRuleQualityConfigurations = configManager.GetQualityConfiguration().RouteRuleQualityConfigurationList;
            if (routeRuleQualityConfigurations == null)
                return null;

            return routeRuleQualityConfigurations.ToDictionary(itm => itm.QualityConfigurationId, itm => itm);
        }

        public RouteRuleQualityConfiguration GetDefaultQualityConfiguration()
        {
            Dictionary<Guid, RouteRuleQualityConfiguration> qualityConfigurations = GetRouteRuleQualityConfigurations();
            if (qualityConfigurations != null)
            {
                foreach (var routeRuleQualityConfigurationKvp in qualityConfigurations)
                {
                    var routeRuleQualityConfiguration = routeRuleQualityConfigurationKvp.Value;
                    if (routeRuleQualityConfiguration.IsDefault)
                    {
                        if (!routeRuleQualityConfiguration.IsActive)
                            throw new VRBusinessException("Default quality configuration is not active");

                        return routeRuleQualityConfiguration;
                    }
                }
            }

            throw new VRBusinessException("No default quality configuration is selected");
        }

        public IEnumerable<QualityConfigurationInfo> GetQualityConfigurationInfo(QualityConfigurationInfoFilter filter)
        {
            Dictionary<Guid, RouteRuleQualityConfiguration> qualityConfigurations = GetRouteRuleQualityConfigurations();
            List<QualityConfigurationInfo> qualityConfigurationInfoList = new List<QualityConfigurationInfo>();

            if (qualityConfigurations != null)
            {
                foreach (var qualityConfigurationKvp in qualityConfigurations)
                {
                    RouteRuleQualityConfiguration qualityConfiguration = qualityConfigurationKvp.Value;

                    QualityConfigurationInfo qualityConfigurationInfo = new QualityConfigurationInfo()
                    {
                        Name = qualityConfiguration.Name,
                        QualityConfigurationId = qualityConfiguration.QualityConfigurationId
                    };

                    qualityConfigurationInfoList.Add(qualityConfigurationInfo);
                }
            }
            return qualityConfigurationInfoList;
        }

        public RouteRuleQualityConfiguration GetQualityConfigurationById(Guid qualityConfigurationId)
        {
            Dictionary<Guid, RouteRuleQualityConfiguration> qualityConfigurations = GetRouteRuleQualityConfigurations();
            if (qualityConfigurations == null)
                return null;

            return qualityConfigurations.GetRecord(qualityConfigurationId);
        }

        public List<RouteRuleQualityConfiguration> GetRouteRuleQualityConfigurationList(List<RouteRule> routeRules)
        {
            List<RouteRuleQualityConfiguration> routeRuleQualityConfigurationList = null;
            List<Guid> qualityConfigurationIds = new List<Guid>();
            Guid defaultConfigurationId = GetDefaultQualityConfiguration().QualityConfigurationId;

            foreach (var routeRule in routeRules)
            {
                RouteRuleQualityContext qualityContext = new RouteRuleQualityContext();
                routeRule.Settings.GetQualityConfigurationIds(qualityContext);

                if (qualityContext.IsDefault)
                    qualityConfigurationIds.Add(defaultConfigurationId);

                if (qualityContext.QualityConfigurationIds != null)
                    qualityConfigurationIds.AddRange(qualityContext.QualityConfigurationIds);
            }

            if (qualityConfigurationIds.Count > 0)
            {
                routeRuleQualityConfigurationList = new List<RouteRuleQualityConfiguration>();
                HashSet<Guid> distinctQualityConfigurationIds = qualityConfigurationIds.ToHashSet();

                foreach (var qualityConfigurationId in distinctQualityConfigurationIds)
                {
                    var qualityConfiguration = GetQualityConfigurationById(qualityConfigurationId);
                    if (qualityConfiguration != null)
                        routeRuleQualityConfigurationList.Add(qualityConfiguration);
                }
            }

            return routeRuleQualityConfigurationList;
        }

        public List<QualityAnalyticRecord> GetQualityAnalyticRecords(RouteRuleQualityConfiguration routeRuleQualityConfiguration, List<string> dimensionFields, List<string> measureFields, 
            Guid analyticTableId, DateTime effectiveDate)
        {
            VRTimePeriodContext vrTimePeriodContext = new VRTimePeriodContext() { EffectiveDate = effectiveDate };

            routeRuleQualityConfiguration.TimePeriod.ThrowIfNull("routeRuleQualityConfiguration.TimePeriod", routeRuleQualityConfiguration.QualityConfigurationId);
            routeRuleQualityConfiguration.TimePeriod.GetTimePeriod(vrTimePeriodContext);

            AnalyticQuery analyticQuery = new AnalyticQuery();
            analyticQuery.FromTime = vrTimePeriodContext.FromTime;
            analyticQuery.ToTime = vrTimePeriodContext.ToTime;
            analyticQuery.TableId = analyticTableId;
            analyticQuery.DimensionFields = dimensionFields;
            analyticQuery.MeasureFields = measureFields;

            AnalyticRecord analyticRecordSummary;
            List<AnalyticRecord> analyticRecords = new AnalyticManager().GetAllFilteredRecords(analyticQuery, out analyticRecordSummary);

            List<QualityAnalyticRecord> qualityAnalyticRecordList = null;

            if (analyticRecords != null)
            {
                qualityAnalyticRecordList = new List<QualityAnalyticRecord>();
                foreach (AnalyticRecord analyticRecord in analyticRecords)
                {
                    QualityAnalyticRecord qualityAnalyticRecord = new QualityAnalyticRecord()
                    {
                        AnalyticRecord = analyticRecord,
                        Quality = CalculateQuality(analyticRecord)
                    };

                    qualityAnalyticRecordList.Add(qualityAnalyticRecord);
                }
            }
            return qualityAnalyticRecordList;
        }

        public Decimal CalculateQuality(AnalyticRecord analyticRecord)
        {
            return new Decimal(new Random().NextDouble());
        }
        #endregion
    }
}