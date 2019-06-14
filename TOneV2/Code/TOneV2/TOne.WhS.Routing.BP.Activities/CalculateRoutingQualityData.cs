﻿using System;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Queueing;

namespace TOne.WhS.Routing.BP.Activities
{
    public class CalculateRoutingQualityDataInput
    {
        public DateTime EffectiveDate { get; set; }

        public RoutingProcessType RoutingProcessType { get; set; }

        public BaseQueue<RouteRuleQualityConfigurationDataBatch> OutputQueue { get; set; }

        public int VersionNumber { get; set; }
    }

    public sealed class CalculateRoutingQualityData : BaseAsyncActivity<CalculateRoutingQualityDataInput>
    {
        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        public InArgument<RoutingProcessType> RoutingProcessType { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<RouteRuleQualityConfigurationDataBatch>> OutputQueue { get; set; }

        [RequiredArgument]
        public InArgument<int> VersionNumber { get; set; }

        protected override void DoWork(CalculateRoutingQualityDataInput inputArgument, AsyncActivityHandle handle)
        {
            DateTime effectiveDate = inputArgument.EffectiveDate;
            RoutingProcessType routingProcessType = inputArgument.RoutingProcessType;

            List<RouteRule> routeRules = new RouteRuleManager().GetEffectiveAndFutureRouteRules(effectiveDate);
            string executionMessage;

            switch (routingProcessType)
            {
                case Entities.RoutingProcessType.CustomerRoute:
                    executionMessage = "Calculate Customer Route Quality Data is done";
                    break;

                case Entities.RoutingProcessType.RoutingProductRoute:
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

                var extendedSettings = qualityConfigurationDefinitionManager.GetQualityConfigurationDefinitionExtendedSettings(qualityConfigurationDefinitionId);

                var initializeQualityConfigurationsContext = new InitializeQualityConfigurationsContext()
                {
                    QualityConfigurationDefinitionId = qualityConfigurationDefinitionId,
                    QualityConfigurations = currentRouteRuleQualityConfigurations
                };
                extendedSettings.InitializeQualityConfigurations(initializeQualityConfigurationsContext);
                Dictionary<Guid, InitializedQualityConfiguration> initializedQualityConfigurationsById = initializeQualityConfigurationsContext.InitializedQualityConfigurationsById;

                foreach (var routeRuleQualityConfiguration in currentRouteRuleQualityConfigurations)
                {
                    routeRuleQualityConfiguration.Settings.ThrowIfNull("routeRuleQualityConfiguration.Settings", routeRuleQualityConfiguration.QualityConfigurationId);

                    GetRouteRuleQualityConfigurationDataContext context = new GetRouteRuleQualityConfigurationDataContext()
                    {
                        QualityConfigurationId = routeRuleQualityConfiguration.QualityConfigurationId,
                        QualityConfigurationDefinitionId = qualityConfigurationDefinitionId,
                        QualityConfigurationName = routeRuleQualityConfiguration.Name,
                        InitializedQualityConfiguration = initializedQualityConfigurationsById.GetRecord(routeRuleQualityConfiguration.QualityConfigurationId),
                        RoutingProcessType = routingProcessType,
                        EffectiveDate = effectiveDate,
                        VersionNumber = inputArgument.VersionNumber
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
                OutputQueue = this.OutputQueue.Get(context),
                VersionNumber = this.VersionNumber.Get(context)
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
