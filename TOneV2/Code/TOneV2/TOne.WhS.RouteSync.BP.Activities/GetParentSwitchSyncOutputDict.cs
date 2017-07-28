using System;
using System.Activities;
using System.Collections.Concurrent;
using TOne.WhS.RouteSync.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.RouteSync.BP.Activities
{
    public sealed class GetParentSwitchSyncOutputDict : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> ParentProcessId { get; set; }

        [RequiredArgument]
        public OutArgument<ConcurrentDictionary<string, SwitchSyncOutput>> ParentSwitchSyncOutputDict { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            ConcurrentDictionary<string, SwitchSyncOutput> parentSwitchSyncOutputDict =
                new Vanrise.Runtime.InterRuntimeServiceManager().SendRequest(this.ParentProcessId.Get(context), new SwitchSyncOutputRequest
                {
                    ProcessInstanceId = context.GetSharedInstanceData().InstanceInfo.ParentProcessID.Value
                });

            ParentSwitchSyncOutputDict.Set(context, parentSwitchSyncOutputDict);
        }
    }
}