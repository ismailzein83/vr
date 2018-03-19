//using System;
//using System.Activities;
//using System.Collections.Generic;
//using System.Linq;
//using TOne.WhS.Routing.Business;
//using TOne.WhS.Routing.Entities;
//using Vanrise.BusinessProcess;
//using Vanrise.Common;
//using Vanrise.Entities;
//using Vanrise.Queueing;

//namespace TOne.WhS.Routing.BP.Activities
//{
//    public class CalculateCustomerRouteQualityDataInput
//    {
//        public DateTime EffectiveDate { get; set; }

//        public BaseQueue<CustomerRouteQualityConfigurationDataBatch> OutputQueue { get; set; }
//    }

//    public sealed class CalculateCustomerRouteQualityData : BaseAsyncActivity<CalculateCustomerRouteQualityDataInput>
//    {
//        [RequiredArgument]
//        public InArgument<DateTime> EffectiveDate { get; set; }

//        [RequiredArgument]
//        public InOutArgument<BaseQueue<CustomerRouteQualityConfigurationDataBatch>> OutputQueue { get; set; }


//        protected override void DoWork(CalculateCustomerRouteQualityDataInput inputArgument, AsyncActivityHandle handle)
//        {
//            DateTime effectiveDate = inputArgument.EffectiveDate;

//            List<RouteRule> customerRouteRules = new RouteRuleManager().GetEffectiveAndFutureCustomerRouteRules(effectiveDate);
//            if (customerRouteRules == null)
//                return;

//            List<RouteRuleQualityConfiguration> routeRuleQualityConfigurations = new QualityConfigurationManager().GetRouteRuleQualityConfigurationList(customerRouteRules);
//            if (routeRuleQualityConfigurations == null)
//                return;

//            Dictionary<Guid, List<RouteRuleQualityConfiguration>> routeRuleQualityConfigurationsByDefId = new Dictionary<Guid, List<RouteRuleQualityConfiguration>>();

//            foreach (var routeRuleQualityConfiguration in routeRuleQualityConfigurations)
//            {
//                List<RouteRuleQualityConfiguration> currentRouteRuleQualityConfigurations = routeRuleQualityConfigurationsByDefId.GetOrCreateItem(routeRuleQualityConfiguration.QualityConfigurationDefinitionId);
//                currentRouteRuleQualityConfigurations.Add(routeRuleQualityConfiguration);
//            }

//            QualityConfigurationDefinitionManager qualityConfigurationDefinitionManager = new QualityConfigurationDefinitionManager();

//            CustomerRouteQualityConfigurationDataBatch batch = new CustomerRouteQualityConfigurationDataBatch();
//            batch.CustomerRouteQualityConfigurationDataList = new List<CustomerRouteQualityConfigurationData>();

//            foreach (var kvp in routeRuleQualityConfigurationsByDefId)
//            {
//                Guid qualityConfigurationDefinitionId = kvp.Key;
//                List<RouteRuleQualityConfiguration> currentRouteRuleQualityConfigurations = kvp.Value;

//                QualityConfigurationDefinitionExtendedSettings extendedSettings = qualityConfigurationDefinitionManager.GetQualityConfigurationDefinitionExtendedSettings(qualityConfigurationDefinitionId);
//                Dictionary<Guid, InitializedQualityConfiguration> initializedQualityConfigurations = extendedSettings.InitializeQualityConfigurations(currentRouteRuleQualityConfigurations);

//                foreach (var routeRuleQualityConfiguration in currentRouteRuleQualityConfigurations)
//                {
//                    routeRuleQualityConfiguration.Settings.ThrowIfNull("routeRuleQualityConfiguration.Settings", routeRuleQualityConfiguration.QualityConfigurationId);

//                    GetRouteRuleQualityConfigurationDataContext context = new GetRouteRuleQualityConfigurationDataContext()
//                    {
//                        QualityConfigurationId = routeRuleQualityConfiguration.QualityConfigurationId,
//                        QualityConfigurationDefinitionId = qualityConfigurationDefinitionId,
//                        QualityConfigurationName = routeRuleQualityConfiguration.Name,
//                        InitializedQualityConfiguration = initializedQualityConfigurations.GetRecord(routeRuleQualityConfiguration.QualityConfigurationId),
//                        RoutingProcessType = RoutingProcessType.CustomerRoute,
//                        EffectiveDate = effectiveDate
//                    };
//                    List<RouteRuleQualityConfigurationData> routeRuleQualityConfigurationData = routeRuleQualityConfiguration.Settings.GetRouteRuleQualityConfigurationData(context);
//                    if (routeRuleQualityConfigurationData == null || routeRuleQualityConfigurationData.Count == 0)
//                        continue;

//                    List<CustomerRouteQualityConfigurationData> customerRouteQualityConfigurationDataList = routeRuleQualityConfigurationData.Select(itm => itm as CustomerRouteQualityConfigurationData).ToList();
//                    batch.CustomerRouteQualityConfigurationDataList.AddRange(customerRouteQualityConfigurationDataList);

//                    //TODO: Batch Count Should be configuration parameter
//                    if (batch.CustomerRouteQualityConfigurationDataList.Count >= 10000)
//                    {
//                        inputArgument.OutputQueue.Enqueue(batch);
//                        batch = new CustomerRouteQualityConfigurationDataBatch();
//                        batch.CustomerRouteQualityConfigurationDataList = new List<CustomerRouteQualityConfigurationData>();
//                    }
//                }
//            }

//            if (batch.CustomerRouteQualityConfigurationDataList.Count > 0)
//                inputArgument.OutputQueue.Enqueue(batch);

//            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Calculate Customer Route Quality Data is done", null);
//        }

//        protected override CalculateCustomerRouteQualityDataInput GetInputArgument(AsyncCodeActivityContext context)
//        {
//            return new CalculateCustomerRouteQualityDataInput
//            {
//                EffectiveDate = this.EffectiveDate.Get(context),
//                OutputQueue = this.OutputQueue.Get(context)
//            };
//        }

//        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
//        {
//            if (this.OutputQueue.Get(context) == null)
//                this.OutputQueue.Set(context, new MemoryQueue<CustomerRouteQualityConfigurationDataBatch>());
//            base.OnBeforeExecute(context, handle);
//        }
//    }
//}