using System;
using System.Activities;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.BusinessProcess;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.BP.Activities;

namespace TOne.WhS.Routing.BP.Activities
{
    public class ApplySwitchRouteSyncRoutesInput
    {
        public SwitchInProcess SwitchInProcess { get; set; }

        public ConcurrentDictionary<string, SwitchSyncOutput> ParentSwitchSyncOutputDict { get; set; }
    }

    public class ApplySwitchRouteSyncRoutesOutput
    {
        public SwitchSyncOutput SwitchSyncOutput { get; set; }
    }

    public sealed class ApplySwitchRouteSyncRoutes : DependentAsyncActivity<ApplySwitchRouteSyncRoutesInput, ApplySwitchRouteSyncRoutesOutput>
    {
        [RequiredArgument]
        public InArgument<SwitchInProcess> SwitchInProcess { get; set; }

        [RequiredArgument]
        public InArgument<ConcurrentDictionary<string, SwitchSyncOutput>> ParentSwitchSyncOutputDict { get; set; }

        [RequiredArgument]
        public OutArgument<SwitchSyncOutput> SwitchSyncOutput { get; set; }

        protected override ApplySwitchRouteSyncRoutesOutput DoWorkWithResult(ApplySwitchRouteSyncRoutesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            var switchInProcess = inputArgument.SwitchInProcess;
            SwitchSyncOutput switchSyncOutput = new SwitchSyncOutput() { SwitchId = inputArgument.SwitchInProcess.Switch.SwitchId, SwitchRouteSynchroniserOutputList = new List<SwitchRouteSynchroniserOutput>(), SwitchSyncResult = SwitchSyncResult.Succeed };

            SwitchSyncOutput previousSwitchSyncOutput = null;
            if (inputArgument.ParentSwitchSyncOutputDict != null)
            {
                var matchedSwitchSyncOutput = inputArgument.ParentSwitchSyncOutputDict.GetRecord(inputArgument.SwitchInProcess.Switch.SwitchId);
                if (matchedSwitchSyncOutput != null)
                    previousSwitchSyncOutput = Vanrise.Common.Utilities.CloneObject<SwitchSyncOutput>(matchedSwitchSyncOutput);
            }

            bool switchFailed = false;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.SwitchInProcess.PreparedRoutesForApplyQueue.TryDequeue((preparedItemsForDBApply) =>
                    {
                        if (!switchFailed)
                        {
                            try
                            {
                                SwitchRouteSynchronizerApplyRoutesContext switchRouteSynchronizerApplyRoutesContext = new SwitchRouteSynchronizerApplyRoutesContext(preparedItemsForDBApply, inputArgument.SwitchInProcess.Switch.Name, inputArgument.SwitchInProcess.Switch.SwitchId, previousSwitchSyncOutput, handle.SharedInstanceData.WriteBusinessHandledException);
                                inputArgument.SwitchInProcess.Switch.RouteSynchronizer.ApplySwitchRouteSyncRoutes(switchRouteSynchronizerApplyRoutesContext);
                                SwitchSyncOutput tempSwitchSyncOutput = switchRouteSynchronizerApplyRoutesContext.SwitchSyncOutput;

                                if (tempSwitchSyncOutput != null)
                                {
                                    switchSyncOutput = TOne.WhS.RouteSync.Business.Helper.MergeSwitchSyncOutputItems(switchSyncOutput, tempSwitchSyncOutput);
                                    previousSwitchSyncOutput = TOne.WhS.RouteSync.Business.Helper.MergeSwitchSyncOutputItems(previousSwitchSyncOutput, tempSwitchSyncOutput);

                                    if (switchSyncOutput.SwitchSyncResult == SwitchSyncResult.Failed)
                                        switchFailed = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                string errorBusinessMessage = Utilities.GetExceptionBusinessMessage(ex);
                                string exceptionDetail = ex.ToString();
                                SwitchRouteSynchroniserOutput output = new SwitchRouteSynchroniserOutput() { ErrorBusinessMessage = errorBusinessMessage, ExceptionDetail = exceptionDetail };
                                switchSyncOutput.SwitchRouteSynchroniserOutputList.Add(output);
                                switchSyncOutput.SwitchSyncResult = SwitchSyncResult.Failed;
                                switchFailed = true;
                                handle.SharedInstanceData.WriteBusinessHandledException(ex);
                            }
                        }
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Applying Codes To Code Sale Zone Table is done", null);

            return new ApplySwitchRouteSyncRoutesOutput { SwitchSyncOutput = switchSyncOutput.SwitchSyncResult != SwitchSyncResult.Succeed ? switchSyncOutput : null };
        }

        protected override ApplySwitchRouteSyncRoutesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplySwitchRouteSyncRoutesInput()
            {
                SwitchInProcess = this.SwitchInProcess.Get(context),
                ParentSwitchSyncOutputDict = this.ParentSwitchSyncOutputDict.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplySwitchRouteSyncRoutesOutput result)
        {
            this.SwitchSyncOutput.Set(context, result.SwitchSyncOutput);
        }
    }
}