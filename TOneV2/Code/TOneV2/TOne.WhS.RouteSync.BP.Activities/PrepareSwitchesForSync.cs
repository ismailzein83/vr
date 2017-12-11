using System.Activities;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Business;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using System.Linq;

namespace TOne.WhS.RouteSync.BP.Activities
{
    public sealed class PrepareSwitchesForSync : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Dictionary<string, SwitchSyncData>> SwitchSyncDataBySwitchId { get; set; }

        [RequiredArgument]
        public InArgument<List<SwitchInfo>> Switches { get; set; }

        [RequiredArgument]
        public InArgument<int> LastVersionNb { get; set; }

        [RequiredArgument]
        public OutArgument<List<SwitchInfo>> PartialRouteSyncSwitches { get; set; }

        [RequiredArgument]
        public OutArgument<List<SwitchInfo>> FullRouteSyncSwitches { get; set; }

        [RequiredArgument]
        public OutArgument<int?> MinVersionNb { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var switchSyncDataBySwitchId = this.SwitchSyncDataBySwitchId.Get(context);
            var switchesInfo = this.Switches.Get(context);
            var lastVersionNb = this.LastVersionNb.Get(context);
            bool executeFullRouteSyncWhenPartialNotSupported = new ConfigManager().GetRouteSyncProcessExecuteFullRouteSyncWhenPartialNotSupported();

            List<string> notSupportingPartialRouteSyncSwitchNames = new List<string>();
            List<SwitchInfo> synchronizedSwitches = new List<SwitchInfo>();
            List<SwitchInfo> partialRouteSyncSwitches = new List<SwitchInfo>();
            List<SwitchInfo> fullRouteSyncSwitches = new List<SwitchInfo>();

            int? minVersionNb = null;
            SwitchSyncData tempSwitchSyncData;

            foreach (var switchInfo in switchesInfo)
            {
                switchInfo.RouteSynchronizer.ThrowIfNull("switchInfo.RouteSynchronizer", switchInfo.SwitchId);
                if (switchSyncDataBySwitchId.TryGetValue(switchInfo.SwitchId, out tempSwitchSyncData))
                {
                    if (switchInfo.RouteSynchronizer.SupportPartialRouteSync)
                    {
                        if (!minVersionNb.HasValue || minVersionNb > tempSwitchSyncData.LastVersionNumber)
                            minVersionNb = tempSwitchSyncData.LastVersionNumber;

                        if (lastVersionNb == tempSwitchSyncData.LastVersionNumber)
                            synchronizedSwitches.Add(switchInfo);
                        else
                            partialRouteSyncSwitches.Add(switchInfo);
                    }
                    else
                    {
                        if (executeFullRouteSyncWhenPartialNotSupported)
                            fullRouteSyncSwitches.Add(switchInfo);
                        else
                            notSupportingPartialRouteSyncSwitchNames.Add(switchInfo.Name);
                    }
                }
                else
                {
                    fullRouteSyncSwitches.Add(switchInfo);
                }
            }

            if (notSupportingPartialRouteSyncSwitchNames.Count > 0)
            {
                string warningMessage = string.Format("Partial Route Sync is not supported for the following switch{0}: '{1}'. To force Full Route Sync, please adjust Route Sync Settings",
                    notSupportingPartialRouteSyncSwitchNames.Count == 1 ? "" : "es", string.Join(", ", notSupportingPartialRouteSyncSwitchNames));
                context.GetSharedInstanceData().WriteTrackingMessage(Vanrise.Entities.LogEntryType.Warning, warningMessage);
            }

            if (synchronizedSwitches.Count > 0)
            {
                string switchSyncMessage = string.Format("Switch{0}: '{1}' already synchronized", synchronizedSwitches.Count == 1 ? "" : "es",
                    string.Join(", ", synchronizedSwitches.Select(itm => itm.Name)));
                context.GetSharedInstanceData().WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, switchSyncMessage);
            }

            this.PartialRouteSyncSwitches.Set(context, partialRouteSyncSwitches);
            this.FullRouteSyncSwitches.Set(context, fullRouteSyncSwitches);
            this.MinVersionNb.Set(context, minVersionNb);
        }
    }
}