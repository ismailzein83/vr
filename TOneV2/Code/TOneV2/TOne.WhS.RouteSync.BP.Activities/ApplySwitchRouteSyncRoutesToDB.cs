using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using TOne.WhS.RouteSync.BP.Activities;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.Routing.BP.Activities
{

    public class ApplySwitchRouteSyncRoutesInput
    {
        public SwitchInProcess SwitchInProcess { get; set; }
    }

    public sealed class ApplySwitchRouteSyncRoutes : DependentAsyncActivity<ApplySwitchRouteSyncRoutesInput>
    {
        [RequiredArgument]
        public InArgument<SwitchInProcess> SwitchInProcess { get; set; }

        protected override void DoWork(ApplySwitchRouteSyncRoutesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            var switchInProcess = inputArgument.SwitchInProcess;

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.SwitchInProcess.PreparedRoutesForApplyQueue.TryDequeue((preparedItemsForDBApply) =>
                    {
                        SwitchRouteSynchronizerApplyRoutesContext switchRouteSynchronizerApplyRoutesContext = new ApplySwitchRouteSyncRoutes.SwitchRouteSynchronizerApplyRoutesContext(preparedItemsForDBApply);
                        inputArgument.SwitchInProcess.Switch.RouteSynchronizer.ApplySwitchRouteSyncRoutes(switchRouteSynchronizerApplyRoutesContext);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Applying Codes To Code Sale Zone Table is done", null);
        }

        protected override ApplySwitchRouteSyncRoutesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplySwitchRouteSyncRoutesInput()
            {
                SwitchInProcess = this.SwitchInProcess.Get(context)
            };
        }

        private class SwitchRouteSynchronizerApplyRoutesContext : ISwitchRouteSynchronizerApplyRoutesContext
        {
            public SwitchRouteSynchronizerApplyRoutesContext(Object preparedItemsForApply)
            {
                PreparedItemsForApply = preparedItemsForApply;
            }
            public Object PreparedItemsForApply { get; set; }
        }
    }
}
