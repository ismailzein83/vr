using System;
using System.Activities;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Vanrise.Common;
using Vanrise.BusinessProcess;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.BP.Activities
{
    public class UpdateAndGetSwitchSyncOutputDict : CodeActivity
    {
        [RequiredArgument]
        public InArgument<ConcurrentDictionary<string, SwitchSyncOutput>> SwitchSyncOutputDict { get; set; }

        public OutArgument<ConcurrentDictionary<string, SwitchSyncOutput>> LatestSwitchSyncOutputDict { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            ConcurrentDictionary<string, SwitchSyncOutput> latestSwitchSyncOutputDict = SwitchSyncOutputRequest.LoadSwitchSyncOutputDict(this.SwitchSyncOutputDict.Get(context), processInstanceId);
            this.LatestSwitchSyncOutputDict.Set(context, latestSwitchSyncOutputDict);
        }
    }

    public class UnloadSwitchSyncOutputDict : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            SwitchSyncOutputRequest.UnloadSwitchSyncOutputDict(processInstanceId);
        }
    }


    public class SwitchSyncOutputRequest : Vanrise.Runtime.Entities.InterRuntimeServiceRequest<ConcurrentDictionary<string, SwitchSyncOutput>>
    {
        static Dictionary<long, ConcurrentDictionary<string, SwitchSyncOutput>> s_switchSyncOutputDictByProcessInstanceId = new Dictionary<long, ConcurrentDictionary<string, SwitchSyncOutput>>();

        public long ProcessInstanceId { get; set; }

        internal static ConcurrentDictionary<string, SwitchSyncOutput> LoadSwitchSyncOutputDict(ConcurrentDictionary<string, SwitchSyncOutput> newSwitchSyncOutputDict, long processInstanceId)
        {
            ConcurrentDictionary<string, SwitchSyncOutput> s_switchSyncOutputDict = s_switchSyncOutputDictByProcessInstanceId.GetOrCreateItem(processInstanceId);

            if (newSwitchSyncOutputDict != null)
            {
                foreach (var newSwitchSyncOutput in newSwitchSyncOutputDict)
                {
                    SwitchSyncOutput currentItem = newSwitchSyncOutput.Value;
                    s_switchSyncOutputDict.AddOrUpdate(currentItem.SwitchId, currentItem, (switchId, switchSyncOutput) =>
                    {
                        currentItem.SwitchSyncResult = (int)switchSyncOutput.SwitchSyncResult > (int)currentItem.SwitchSyncResult ? switchSyncOutput.SwitchSyncResult : currentItem.SwitchSyncResult;
                        if (switchSyncOutput.SwitchRouteSynchroniserOutputList != null && switchSyncOutput.SwitchRouteSynchroniserOutputList.Count > 0)
                        {
                            if (currentItem.SwitchRouteSynchroniserOutputList == null)
                                currentItem.SwitchRouteSynchroniserOutputList = new List<SwitchRouteSynchroniserOutput>();

                            currentItem.SwitchRouteSynchroniserOutputList.AddRange(switchSyncOutput.SwitchRouteSynchroniserOutputList);
                        }
                        return currentItem;
                    });
                }
            }
            return s_switchSyncOutputDict;
        }

        internal static void UnloadSwitchSyncOutputDict(long processInstanceId)
        {
            s_switchSyncOutputDictByProcessInstanceId.Remove(processInstanceId);
        }

        public override ConcurrentDictionary<string, SwitchSyncOutput> Execute()
        {
            return s_switchSyncOutputDictByProcessInstanceId.GetRecord(this.ProcessInstanceId);
        }
    }
}