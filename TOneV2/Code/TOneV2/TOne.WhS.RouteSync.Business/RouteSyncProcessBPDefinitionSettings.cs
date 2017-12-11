using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.RouteSync.BP.Arguments;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.Routing.BP.Arguments;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Business
{
    public class RouteSyncProcessBPDefinitionSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(Vanrise.BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            RouteSyncProcessInput routeSyncProcessInputArg = context.IntanceToRun.InputArgument.CastWithValidate<RouteSyncProcessInput>("context.IntanceToRun.InputArgument");

            RouteSyncDefinitionManager routeSyncDefinitionManager = new RouteSyncDefinitionManager();
            RouteSyncDefinition routeSyncDefinition = routeSyncDefinitionManager.GetRouteSyncDefinitionById(routeSyncProcessInputArg.RouteSyncDefinitionId);
            routeSyncDefinition.ThrowIfNull("routeSyncDefinition", routeSyncProcessInputArg.RouteSyncDefinitionId);
            routeSyncDefinition.Settings.ThrowIfNull("routeSyncDefinition.Settings", routeSyncProcessInputArg.RouteSyncDefinitionId);
            routeSyncDefinition.Settings.SwitchIds.ThrowIfNull("routeSyncDefinition.Settings.SwitchIds", routeSyncProcessInputArg.RouteSyncDefinitionId);

            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                RouteSyncProcessInput startedBPInstanceRouteSyncArg = startedBPInstance.InputArgument as RouteSyncProcessInput;
                if (startedBPInstanceRouteSyncArg != null)
                {
                    if (routeSyncProcessInputArg.RouteSyncDefinitionId == startedBPInstanceRouteSyncArg.RouteSyncDefinitionId)
                    {
                        context.Reason = "Another Route Sync instance for same Route Sync Definition is running";
                        return false;
                    }

                    RouteSyncDefinition startedRouteSyncDefinition = routeSyncDefinitionManager.GetRouteSyncDefinitionById(startedBPInstanceRouteSyncArg.RouteSyncDefinitionId);
                    startedRouteSyncDefinition.ThrowIfNull("startedRouteSyncDefinition", startedBPInstanceRouteSyncArg.RouteSyncDefinitionId);
                    startedRouteSyncDefinition.Settings.ThrowIfNull("startedRouteSyncDefinition.Settings", startedBPInstanceRouteSyncArg.RouteSyncDefinitionId);
                    startedRouteSyncDefinition.Settings.SwitchIds.ThrowIfNull("startedRouteSyncDefinition.Settings.SwitchIds", startedBPInstanceRouteSyncArg.RouteSyncDefinitionId);

                    if (routeSyncDefinition.Settings.SwitchIds.Any(startedRouteSyncDefinition.Settings.SwitchIds.Contains))
                    {
                        context.Reason = "Another Route Sync instance for same switch(es) is running";
                        return false;
                    }
                }

                RoutingProcessInput startedBPInstanceRoutingArg = startedBPInstance.InputArgument as RoutingProcessInput;
                if (startedBPInstanceRoutingArg != null && startedBPInstanceRoutingArg.RoutingDatabaseType == RoutingDatabaseType.Current)
                {
                    context.Reason = "Route Build instance of type Current is running";
                    return false;
                }
            }

            return true;
        }

        public override bool ShouldCreateScheduledInstance(Vanrise.BusinessProcess.Entities.IBPDefinitionShouldCreateScheduledInstanceContext context)
        {
            RoutingDatabase routingDatabase = new RoutingDatabaseManager().GetLatestRoutingDatabase(RoutingProcessType.CustomerRoute, RoutingDatabaseType.Current);
            if (routingDatabase == null)
                return false;

            RouteSyncProcessInput routeSyncProcessInputArg = context.BaseProcessInputArgument.CastWithValidate<RouteSyncProcessInput>("context.BaseProcessInputArgument");

            RouteSyncDefinition routeSyncDefinition = new RouteSyncDefinitionManager().GetRouteSyncDefinitionById(routeSyncProcessInputArg.RouteSyncDefinitionId);
            routeSyncDefinition.ThrowIfNull("routeSyncDefinition", routeSyncProcessInputArg.RouteSyncDefinitionId);
            routeSyncDefinition.Settings.ThrowIfNull("routeSyncDefinition.Settings", routeSyncProcessInputArg.RouteSyncDefinitionId);
            routeSyncDefinition.Settings.SwitchIds.ThrowIfNull("routeSyncDefinition.Settings.SwitchIds", routeSyncProcessInputArg.RouteSyncDefinitionId);

            IRoutingEntityDetailsDataManager routingEntityDetailsDataManager = RoutingDataManagerFactory.GetDataManager<IRoutingEntityDetailsDataManager>();
            routingEntityDetailsDataManager.RoutingDatabase = routingDatabase;
            RoutingEntityDetails routingEntityDetails = routingEntityDetailsDataManager.GetRoutingEntityDetails(RoutingEntityType.PartialRouteInfo);
            routingEntityDetails.ThrowIfNull("routingEntityDetails");
            PartialRouteInfo partialRouteInfo = routingEntityDetails.RoutingEntityInfo.CastWithValidate<PartialRouteInfo>("routingEntityDetails.RoutingEntityInfo");

            ISwitchSyncDataDataManager switchSyncDataDataManager = RoutingDataManagerFactory.GetDataManager<ISwitchSyncDataDataManager>();
            switchSyncDataDataManager.RoutingDatabase = routingDatabase;
            List<SwitchSyncData> switchSyncDataList = switchSyncDataDataManager.GetSwitchSyncDataByIds(routeSyncDefinition.Settings.SwitchIds);
            Dictionary<string, SwitchSyncData> switchSyncDataDict = switchSyncDataList.ToDictionary(itm => itm.SwitchId, itm => itm);

            var switchManager = new TOne.WhS.BusinessEntity.Business.SwitchManager();
            bool executeFullRouteSyncWhenPartialNotSupported = new ConfigManager().GetRouteSyncProcessExecuteFullRouteSyncWhenPartialNotSupported(); 
 
            SwitchSyncData currentSwitchSyncData;

            foreach (var switchId in routeSyncDefinition.Settings.SwitchIds)
            {
                if (!switchSyncDataDict.TryGetValue(switchId, out currentSwitchSyncData))
                    return true;

                if (currentSwitchSyncData.LastVersionNumber >= partialRouteInfo.LastVersionNumber)
                    continue;

                Switch currentSwitch = switchManager.GetSwitch(Convert.ToInt32(switchId));
                if (currentSwitch.Settings.RouteSynchronizer.SupportPartialRouteSync || executeFullRouteSyncWhenPartialNotSupported)
                    return true;
            }

            return false;
        }
    }
}