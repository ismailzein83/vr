using System;
using System.Activities;
using System.Collections.Concurrent;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Business;
using System.Collections.Generic;
using System.Linq;

namespace TOne.WhS.RouteSync.BP.Activities
{
    #region Argument Classes

    public class LogFailedSwitchRouteSyncInput
    {
        public ConcurrentDictionary<string, SwitchSyncOutput> SwitchSyncOutputDict { get; set; }
    }

    public class LogFailedSwitchRouteSyncOutput
    {

    }

    #endregion

    public sealed class LogFailedSwitchRouteSync : BaseAsyncActivity<LogFailedSwitchRouteSyncInput, LogFailedSwitchRouteSyncOutput>
    {
        [RequiredArgument]
        public InArgument<ConcurrentDictionary<string, SwitchSyncOutput>> SwitchSyncOutputDict { get; set; }

        protected override LogFailedSwitchRouteSyncOutput DoWorkWithResult(LogFailedSwitchRouteSyncInput inputArgument, AsyncActivityHandle handle)
        {
            if (inputArgument.SwitchSyncOutputDict != null)
            {
                HashSet<string> loggedMessagesKey = new HashSet<string>();
                SwitchManager switchManager = new SwitchManager();
                foreach (var switchSyncOutput in inputArgument.SwitchSyncOutputDict.Values)
                {
                    if (switchSyncOutput.SwitchSyncResult == SwitchSyncResult.Succeed)
                        continue;

                    SwitchInfo switchInfo = switchManager.GetSwitches(new List<string>() { switchSyncOutput.SwitchId }).FirstOrDefault();
                    if (switchSyncOutput.SwitchRouteSynchroniserOutputList == null || switchSyncOutput.SwitchRouteSynchroniserOutputList.Count == 0)
                    {
                        handle.SharedInstanceData.WriteBusinessTrackingMsg(LogEntryType.Warning, "Switch '{0}' is not synchronised", switchInfo.Name);
                    }
                    else
                    {
                        foreach (SwitchRouteSynchroniserOutput switchRouteSynchroniserOutput in switchSyncOutput.SwitchRouteSynchroniserOutputList)
                        {
                            string msgUniqueKey = switchRouteSynchroniserOutput.GetUniqueMessageKey(switchSyncOutput.SwitchId);
                            if (!loggedMessagesKey.Contains(msgUniqueKey))
                            {
                                switchRouteSynchroniserOutput.LogMessage(switchInfo.Name, handle.SharedInstanceData.WriteBusinessHandledException);
                                loggedMessagesKey.Add(msgUniqueKey);
                            }
                        }
                    }
                }
            }

            return new LogFailedSwitchRouteSyncOutput();
        }

        protected override LogFailedSwitchRouteSyncInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LogFailedSwitchRouteSyncInput()
            {
                SwitchSyncOutputDict = this.SwitchSyncOutputDict.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LogFailedSwitchRouteSyncOutput result)
        {

        }
    }
}