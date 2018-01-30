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
using Vanrise.Queueing;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Routing.BP.Activities
{
    public class CalculateRPQualityDataInput
    {
        public DateTime EffectiveDate { get; set; }
        public BaseQueue<RPQualityConfigurationDataBatch> OutputQueue { get; set; }
    }

    public sealed class CalculateRPQualityData : BaseAsyncActivity<CalculateRPQualityDataInput>
    {
        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<RPQualityConfigurationDataBatch>> OutputQueue { get; set; }


        protected override void DoWork(CalculateRPQualityDataInput inputArgument, AsyncActivityHandle handle)
        {
            DateTime effectiveDate = inputArgument.EffectiveDate;

            List<RouteRule> rpRouteRules = new RouteRuleManager().GetEffectiveAndFutureRPRouteRules(effectiveDate);
            if (rpRouteRules == null)
                return;

            QualityConfigurationManager qualityDataManager = new QualityConfigurationManager();

            List<RouteRuleQualityConfiguration> routeRuleQualityConfigurationList = qualityDataManager.GetRouteRuleQualityConfigurationList(rpRouteRules);
            if (routeRuleQualityConfigurationList == null)
                return;

            List<string> dimensionFields = new List<string>() { "SaleZone", "Supplier" };
            Dictionary<Guid, List<string>> measureFieldsByQualityConfigurationId = qualityDataManager.GetQualityConfigurationExpressionsMeasureFields(routeRuleQualityConfigurationList);

            Guid analyticTableId = new ConfigManager().GetQualityAnalyticTableId();
            var routeRuleQualityConfigurationDataList = qualityDataManager.GetRouteRuleQualityConfigurationDataList(routeRuleQualityConfigurationList);
            if (routeRuleQualityConfigurationDataList == null)
                return;

            RPQualityConfigurationDataBatch batch = new RPQualityConfigurationDataBatch();
            batch.RPQualityConfigurationDataList = new List<RPQualityConfigurationData>();

            foreach (var routeRuleQualityConfigurationData in routeRuleQualityConfigurationDataList)
            {
                List<string> measureFields = null;
                if (measureFieldsByQualityConfigurationId != null)
                    measureFieldsByQualityConfigurationId.TryGetValue(routeRuleQualityConfigurationData.Entity.QualityConfigurationId, out measureFields);

                var qualityAnalyticRecordList = qualityDataManager.GetQualityAnalyticRecords(routeRuleQualityConfigurationData, analyticTableId, dimensionFields, measureFields, effectiveDate);
                if (qualityAnalyticRecordList == null)
                    continue;

                foreach (var qualityAnalyticRecord in qualityAnalyticRecordList)
                {
                    DimensionValue currentSaleZone = qualityAnalyticRecord.AnalyticRecord.DimensionValues[0];
                    DimensionValue currentSupplier = qualityAnalyticRecord.AnalyticRecord.DimensionValues[1];

                    if (currentSaleZone.Value == null || currentSupplier.Value == null)
                        continue;

                    long saleZoneId = (long)currentSaleZone.Value;
                    int supplierId = (int)currentSupplier.Value;

                    batch.RPQualityConfigurationDataList.Add(new RPQualityConfigurationData()
                    {
                        QualityConfigurationId = routeRuleQualityConfigurationData.Entity.QualityConfigurationId,
                        SaleZoneId = saleZoneId,
                        SupplierId = supplierId,
                        QualityData = qualityAnalyticRecord.Quality
                    });

                    //TODO: Batch Count Should be configuration parameter
                    if (batch.RPQualityConfigurationDataList.Count >= 10000)
                    {
                        inputArgument.OutputQueue.Enqueue(batch);
                        batch = new RPQualityConfigurationDataBatch();
                        batch.RPQualityConfigurationDataList = new List<RPQualityConfigurationData>();
                    }
                }
            }

            if (batch.RPQualityConfigurationDataList.Count > 0)
                inputArgument.OutputQueue.Enqueue(batch);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Calculate Routing Product Quality Data is done", null);
        }

        protected override CalculateRPQualityDataInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new CalculateRPQualityDataInput
            {
                EffectiveDate = this.EffectiveDate.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<RPQualityConfigurationDataBatch>());
            base.OnBeforeExecute(context, handle);
        }
    }
}