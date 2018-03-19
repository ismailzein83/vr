using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Routing.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Business;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public class CalculateRoutingQualityDataInput
    {
        public DateTime EffectiveDate { get; set; }

        public RoutingProcessType RoutingProcessType { get; set; }

        public BaseQueue<RouteRuleQualityConfigurationDataBatch> OutputQueue { get; set; }
    }

    public sealed class CalculateRoutingQualityData : BaseAsyncActivity<CalculateRoutingQualityDataInput>
    {
        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        public InArgument<RoutingProcessType> RoutingProcessType { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<RouteRuleQualityConfigurationDataBatch>> OutputQueue { get; set; }


        protected override void DoWork(CalculateRoutingQualityDataInput inputArgument, AsyncActivityHandle handle)
        {
            DateTime effectiveDate = inputArgument.EffectiveDate;
            RoutingProcessType routingProcessType = inputArgument.RoutingProcessType;

            List<RouteRule> routeRules = null;
            string executionMessage;

            switch (routingProcessType)
            {
                case Entities.RoutingProcessType.CustomerRoute:
                    routeRules = new RouteRuleManager().GetEffectiveAndFutureCustomerRouteRules(effectiveDate);
                    executionMessage = "Calculate Customer Route Quality Data is done";
                    break;

                case Entities.RoutingProcessType.RoutingProductRoute:
                    routeRules = new RouteRuleManager().GetEffectiveAndFutureRPRouteRules(effectiveDate);
                    executionMessage = "Calculate Routing Product Quality Data is done";
                    break;

                default: throw new Exception(string.Format("Unsupported RoutingProcessType: {0}", routingProcessType));
            }

            if (routeRules == null)
                return;

            List<RouteRuleQualityConfiguration> routeRuleQualityConfigurations = new QualityConfigurationManager().GetRouteRuleQualityConfigurationList(routeRules);
            if (routeRuleQualityConfigurations == null)
                return;

            Dictionary<Guid, List<RouteRuleQualityConfiguration>> routeRuleQualityConfigurationsByDefId = new Dictionary<Guid, List<RouteRuleQualityConfiguration>>();

            foreach (var routeRuleQualityConfiguration in routeRuleQualityConfigurations)
            {
                List<RouteRuleQualityConfiguration> currentRouteRuleQualityConfigurations = routeRuleQualityConfigurationsByDefId.GetOrCreateItem(routeRuleQualityConfiguration.QualityConfigurationDefinitionId);
                currentRouteRuleQualityConfigurations.Add(routeRuleQualityConfiguration);
            }

            QualityConfigurationDefinitionManager qualityConfigurationDefinitionManager = new QualityConfigurationDefinitionManager();

            RouteRuleQualityConfigurationDataBatch batch = new RouteRuleQualityConfigurationDataBatch();
            batch.RoutingQualityConfigurationDataList = new List<RouteRuleQualityConfigurationData>();

            foreach (var kvp in routeRuleQualityConfigurationsByDefId)
            {
                Guid qualityConfigurationDefinitionId = kvp.Key;
                List<RouteRuleQualityConfiguration> currentRouteRuleQualityConfigurations = kvp.Value;

                QualityConfigurationDefinitionExtendedSettings extendedSettings = qualityConfigurationDefinitionManager.GetQualityConfigurationDefinitionExtendedSettings(qualityConfigurationDefinitionId);
                Dictionary<Guid, InitializedQualityConfiguration> initializedQualityConfigurations = extendedSettings.InitializeQualityConfigurations(currentRouteRuleQualityConfigurations);

                foreach (var routeRuleQualityConfiguration in currentRouteRuleQualityConfigurations)
                {
                    routeRuleQualityConfiguration.Settings.ThrowIfNull("routeRuleQualityConfiguration.Settings", routeRuleQualityConfiguration.QualityConfigurationId);

                    GetRouteRuleQualityConfigurationDataContext context = new GetRouteRuleQualityConfigurationDataContext()
                    {
                        QualityConfigurationId = routeRuleQualityConfiguration.QualityConfigurationId,
                        QualityConfigurationDefinitionId = qualityConfigurationDefinitionId,
                        QualityConfigurationName = routeRuleQualityConfiguration.Name,
                        InitializedQualityConfiguration = initializedQualityConfigurations.GetRecord(routeRuleQualityConfiguration.QualityConfigurationId),
                        RoutingProcessType = routingProcessType,
                        EffectiveDate = effectiveDate
                    };
                    List<RouteRuleQualityConfigurationData> routeRuleQualityConfigurationData = routeRuleQualityConfiguration.Settings.GetRouteRuleQualityConfigurationData(context);
                    if (routeRuleQualityConfigurationData == null || routeRuleQualityConfigurationData.Count == 0)
                        continue;

                    batch.RoutingQualityConfigurationDataList.AddRange(routeRuleQualityConfigurationData);

                    //TODO: Batch Count Should be configuration parameter
                    if (batch.RoutingQualityConfigurationDataList.Count >= 10000)
                    {
                        inputArgument.OutputQueue.Enqueue(batch);
                        batch = new RouteRuleQualityConfigurationDataBatch();
                        batch.RoutingQualityConfigurationDataList = new List<RouteRuleQualityConfigurationData>();
                    }
                }
            }

            if (batch.RoutingQualityConfigurationDataList.Count > 0)
                inputArgument.OutputQueue.Enqueue(batch);

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, executionMessage, null);
        }

        protected override CalculateRoutingQualityDataInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new CalculateRoutingQualityDataInput
            {
                EffectiveDate = this.EffectiveDate.Get(context),
                RoutingProcessType = this.RoutingProcessType.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<RouteRuleQualityConfigurationDataBatch>());
            base.OnBeforeExecute(context, handle);
        }
    }
}
