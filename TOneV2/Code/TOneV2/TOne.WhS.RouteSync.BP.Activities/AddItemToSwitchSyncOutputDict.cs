using System;
using System.Activities;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Vanrise.BusinessProcess;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.BP.Activities
{
    public class AddItemToSwitchSyncOutputInput
    {
        public SwitchSyncOutput SwitchSyncOutput { get; set; }

        public ConcurrentDictionary<string, SwitchSyncOutput> SwitchSyncOutputDict { get; set; }
    }

    public class AddItemToSwitchSyncOutputOutput
    {

    }

    public sealed class AddItemToSwitchSyncOutputDict : DependentAsyncActivity<AddItemToSwitchSyncOutputInput, AddItemToSwitchSyncOutputOutput>
    {
        [RequiredArgument]
        public InArgument<ConcurrentDictionary<string, SwitchSyncOutput>> SwitchSyncOutputDict { get; set; }

        [RequiredArgument]
        public InArgument<SwitchSyncOutput> SwitchSyncOutput { get; set; }

        protected override AddItemToSwitchSyncOutputOutput DoWorkWithResult(AddItemToSwitchSyncOutputInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            inputArgument.SwitchSyncOutputDict.AddOrUpdate(inputArgument.SwitchSyncOutput.SwitchId, inputArgument.SwitchSyncOutput, (switchId, switchSyncOutput) =>
            {
                switchSyncOutput.SwitchSyncResult = (int)switchSyncOutput.SwitchSyncResult > (int)switchSyncOutput.SwitchSyncResult ? switchSyncOutput.SwitchSyncResult : switchSyncOutput.SwitchSyncResult;
                if (switchSyncOutput.SwitchRouteSynchroniserOutputList != null && switchSyncOutput.SwitchRouteSynchroniserOutputList.Count > 0)
                {
                    if (switchSyncOutput.SwitchRouteSynchroniserOutputList == null)
                        switchSyncOutput.SwitchRouteSynchroniserOutputList = new List<SwitchRouteSynchroniserOutput>();

                    switchSyncOutput.SwitchRouteSynchroniserOutputList.AddRange(switchSyncOutput.SwitchRouteSynchroniserOutputList);
                }
                return switchSyncOutput;
            });
            return new AddItemToSwitchSyncOutputOutput() { };
        }

        protected override AddItemToSwitchSyncOutputInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new AddItemToSwitchSyncOutputInput()
            {
                SwitchSyncOutputDict = this.SwitchSyncOutputDict.Get(context),
                SwitchSyncOutput = this.SwitchSyncOutput.Get(context),
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, AddItemToSwitchSyncOutputOutput result)
        {

        }
    }
}