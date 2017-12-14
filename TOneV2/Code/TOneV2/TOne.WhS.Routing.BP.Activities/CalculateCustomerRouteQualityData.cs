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
    public sealed class CalculateCustomerQualityData : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<int> Routingdatabaseid { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int routingdatabaseid = this.Routingdatabaseid.Get(context);
            DateTime effectiveDate = this.EffectiveDate.Get(context);

            QualityConfigurationManager qualityDataManager = new QualityConfigurationManager();
            ICustomerQualityConfigurationDataManager customerQualityConfigurationDataManager = RoutingDataManagerFactory.GetDataManager<ICustomerQualityConfigurationDataManager>();
            customerQualityConfigurationDataManager.RoutingDatabase = new RoutingDatabaseManager().GetRoutingDatabase(routingdatabaseid);

            List<string> dimensionFields = new List<string>() { "SupplierZone" };
            List<string> measureFields = new List<string>() { "ASR" };

            List<RouteRule> customerRouteRules = new RouteRuleManager().GetEffectiveCustomerRouteRules(effectiveDate);
            if (customerRouteRules == null)
                return;

            var dbApplyStream = customerQualityConfigurationDataManager.InitialiazeStreamForDBApply();

            List<RouteRuleQualityConfiguration> routeRuleQualityConfigurationList = qualityDataManager.GetRouteRuleQualityConfigurationList(customerRouteRules);
            if (routeRuleQualityConfigurationList == null)
                return;

            long? supplierZoneId = null;
            Decimal qualityDataResult;

            Guid analyticTableId = new ConfigManager().GetQualityAnalyticTableId();

            List<QualityAnalyticRecord> qualityAnalyticRecordList;

            foreach (var routeRuleQualityConfiguration in routeRuleQualityConfigurationList)
            {
                qualityAnalyticRecordList = qualityDataManager.GetQualityAnalyticRecords(routeRuleQualityConfiguration, dimensionFields, measureFields, analyticTableId, effectiveDate);

                foreach (var qualityAnalyticRecord in qualityAnalyticRecordList)
                {
                    foreach (var dimensionValue in qualityAnalyticRecord.AnalyticRecord.DimensionValues)
                    {
                        if (dimensionValue.Value == null)
                            continue;

                        switch (dimensionValue.Name)
                        {
                            case "SupplierZone": supplierZoneId = (long)dimensionValue.Value; break;
                            default: break;
                        }
                    }

                    qualityDataResult = qualityAnalyticRecord.Quality;
                    CustomerRouteQualityConfigurationData customerQualityConfigurationDataInfo = new CustomerRouteQualityConfigurationData()
                    {
                        QualityConfigurationId = routeRuleQualityConfiguration.QualityConfigurationId,
                        SupplierZoneId = supplierZoneId,
                        QualityData = qualityDataResult
                    };
                    customerQualityConfigurationDataManager.WriteRecordToStream(customerQualityConfigurationDataInfo, dbApplyStream);
                }
            }

            var streamReadyToApply = customerQualityConfigurationDataManager.FinishDBApplyStream(dbApplyStream);
            customerQualityConfigurationDataManager.ApplyQualityConfigurationsToDB(streamReadyToApply);
        }
    }
}