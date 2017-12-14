using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using TOne.WhS.Routing.Data;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class CalculateRPQualityData : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<int> Routingdatabaseid { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            RouteRuleManager routeRuleManager = new RouteRuleManager();
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            QualityConfigurationManager qualityConfigurationManager = new QualityConfigurationManager();
            IRPQualityConfigurationDataManager rpQualityConfigurationDataManager = RoutingDataManagerFactory.GetDataManager<IRPQualityConfigurationDataManager>();

            DateTime effectiveDate = this.EffectiveDate.Get(context);
            int routingdatabaseid = this.Routingdatabaseid.Get(context);
            List<string> dimensionFields = new List<string>() { "Supplier", "SaleZone" };
            List<string> measureFields = new List<string>() { "ASR" };

            rpQualityConfigurationDataManager.RoutingDatabase = routingDatabaseManager.GetRoutingDatabase(routingdatabaseid);

            List<RouteRule> rpRouteRules = routeRuleManager.GetEffectiveRPRouteRules(effectiveDate);
            if (rpRouteRules == null)
                return;

            var dbApplyStream = rpQualityConfigurationDataManager.InitialiazeStreamForDBApply();

            List<RouteRuleQualityConfiguration> routeRuleQualityConfigurationList = qualityConfigurationManager.GetRouteRuleQualityConfigurationList(rpRouteRules);

            if (routeRuleQualityConfigurationList == null)
                return;

            int? supplierId = null;
            long? salezoneId = null;

            Guid analyticTableId = new ConfigManager().GetQualityAnalyticTableId();

            List<QualityAnalyticRecord> qualityAnalyticRecordList;

            foreach (var routeRuleQualityConfiguration in routeRuleQualityConfigurationList)
            {
                qualityAnalyticRecordList = qualityConfigurationManager.GetQualityAnalyticRecords(routeRuleQualityConfiguration, dimensionFields, measureFields, analyticTableId, effectiveDate);

                foreach (var qualityAnalyticRecord in qualityAnalyticRecordList)
                {
                    foreach (var dimensionValue in qualityAnalyticRecord.AnalyticRecord.DimensionValues)
                    {
                        if (dimensionValue.Value == null)
                            continue;

                        switch (dimensionValue.Name)
                        {
                            case "Supplier": supplierId = (int)dimensionValue.Value; break;
                            case "SaleZone": salezoneId = (long)dimensionValue.Value; break;
                            default: break;
                        }
                    }

                    RPQualityConfigurationData rpQualityConfigurationData = new RPQualityConfigurationData()
                    {
                        QualityConfigurationId = routeRuleQualityConfiguration.QualityConfigurationId,
                        SupplierId = supplierId,
                        SaleZoneId = salezoneId,
                        QualityData = qualityAnalyticRecord.Quality
                    };
                    rpQualityConfigurationDataManager.WriteRecordToStream(rpQualityConfigurationData, dbApplyStream);
                }

                var streamReadyToApply = rpQualityConfigurationDataManager.FinishDBApplyStream(dbApplyStream);
                rpQualityConfigurationDataManager.ApplyQualityConfigurationsToDB(streamReadyToApply);

            }
        }
    }
}
