using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.Queueing;

namespace TOne.WhS.Routing.BP.Activities
{
    public class CalculateCustomerRouteQualityDataInput
    {
        public DateTime EffectiveDate { get; set; }
        public BaseQueue<CustomerRouteQualityConfigurationDataBatch> OutputQueue { get; set; }
    }

    public sealed class CalculateCustomerRouteQualityData : BaseAsyncActivity<CalculateCustomerRouteQualityDataInput>
    {
        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<CustomerRouteQualityConfigurationDataBatch>> OutputQueue { get; set; }

        protected override void DoWork(CalculateCustomerRouteQualityDataInput inputArgument, AsyncActivityHandle handle)
        {
            DateTime effectiveDate = inputArgument.EffectiveDate;

            List<RouteRule> customerRouteRules = new RouteRuleManager().GetEffectiveAndFutureCustomerRouteRules(effectiveDate);
            if (customerRouteRules == null)
                return;

            QualityConfigurationManager qualityDataManager = new QualityConfigurationManager();

            List<RouteRuleQualityConfiguration> routeRuleQualityConfigurationList = qualityDataManager.GetRouteRuleQualityConfigurationList(customerRouteRules);
            if (routeRuleQualityConfigurationList == null)
                return;

            List<string> dimensionFields = new List<string>() { "SupplierZone" };
            Dictionary<Guid, List<string>> measureFieldsByQualityConfigurationId = qualityDataManager.GetQualityConfigurationExpressionsMeasureFields(routeRuleQualityConfigurationList);

            Guid analyticTableId = new ConfigManager().GetQualityAnalyticTableId();
            var routeRuleQualityConfigurationDataList = qualityDataManager.GetRouteRuleQualityConfigurationDataList(routeRuleQualityConfigurationList);

            CustomerRouteQualityConfigurationDataBatch batch = new CustomerRouteQualityConfigurationDataBatch();
            batch.CustomerRouteQualityConfigurationDataList = new List<CustomerRouteQualityConfigurationData>();

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
                    var dimensionValue = qualityAnalyticRecord.AnalyticRecord.DimensionValues.First();
                    if (dimensionValue.Value == null)
                        continue;

                    long supplierZoneId = (long)dimensionValue.Value;

                    batch.CustomerRouteQualityConfigurationDataList.Add(new CustomerRouteQualityConfigurationData()
                    {
                        QualityConfigurationId = routeRuleQualityConfigurationData.Entity.QualityConfigurationId,
                        SupplierZoneId = supplierZoneId,
                        QualityData = qualityAnalyticRecord.Quality
                    });

                    //TODO: Batch Count Should be configuration parameter
                    if (batch.CustomerRouteQualityConfigurationDataList.Count >= 10000)
                    {
                        inputArgument.OutputQueue.Enqueue(batch);
                        batch = new CustomerRouteQualityConfigurationDataBatch();
                        batch.CustomerRouteQualityConfigurationDataList = new List<CustomerRouteQualityConfigurationData>();
                    }
                }
            }

            if (batch.CustomerRouteQualityConfigurationDataList.Count > 0)
                inputArgument.OutputQueue.Enqueue(batch);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Calculate Customer Route Quality Data is done", null);
        }

        protected override CalculateCustomerRouteQualityDataInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new CalculateCustomerRouteQualityDataInput
            {
                EffectiveDate = this.EffectiveDate.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<CustomerRouteQualityConfigurationDataBatch>());
            base.OnBeforeExecute(context, handle);
        }
    }
}