using System;
using System.Collections.Generic;
using System.Activities;
using TOne.WhS.RouteSync.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TOne.WhS.RouteSync.BP.Activities
{
    #region Argument Classes

    public class PrepareSwitchRouteSyncRoutesForApplyInput
    {
        public RouteRangeType? RangeType { get; set; }

        public RouteRangeInfo RangeInfo { get; set; }

        public SwitchInProcess SwitchInProcess { get; set; }
    }

    public class PrepareSwitchRouteSyncRoutesForApplyOutput
    {

    }

    #endregion

    public sealed class PrepareSwitchRouteSyncRoutesForApply : DependentAsyncActivity<PrepareSwitchRouteSyncRoutesForApplyInput, PrepareSwitchRouteSyncRoutesForApplyOutput>
    {
        [RequiredArgument]
        public InArgument<RouteRangeType?> RangeType { get; set; }

        [RequiredArgument]
        public InArgument<RouteRangeInfo> RangeInfo { get; set; }

        [RequiredArgument]
        public InArgument<SwitchInProcess> SwitchInProcess { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            var switcheInProcess = this.SwitchInProcess.Get(context);
            if (switcheInProcess.PreparedRoutesForApplyQueue == null)
                switcheInProcess.PreparedRoutesForApplyQueue = new MemoryQueue<Object>();
            base.OnBeforeExecute(context, handle);
        }

        protected override PrepareSwitchRouteSyncRoutesForApplyOutput DoWorkWithResult(PrepareSwitchRouteSyncRoutesForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            int totalBatchesUpdated = 0;
            long totalRoutesUpdated = 0;
            var switchInProcess = inputArgument.SwitchInProcess;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.SwitchInProcess.ConvertedRouteQueue.TryDequeue((convertedRouteBatch) =>
                    {
                        SwitchRouteSynchronizerPrepareDataForApplyContext switchRouteSynchronizerPrepareDataForApplyContext = new SwitchRouteSynchronizerPrepareDataForApplyContext
                        {
                            ConvertedRoutes = convertedRouteBatch.ConvertedRoutes,
                            RouteRangeType = inputArgument.RangeType,
                            RouteRangeInfo = inputArgument.RangeInfo,
                            InitializationData = switchInProcess.InitializationData,
							SwitchId = inputArgument.SwitchInProcess.Switch.SwitchId
                        };
                        Object preparedItemsForDBApply = switchInProcess.Switch.RouteSynchronizer.PrepareDataForApply(switchRouteSynchronizerPrepareDataForApplyContext);
                        inputArgument.SwitchInProcess.PreparedRoutesForApplyQueue.Enqueue(preparedItemsForDBApply);
                        totalBatchesUpdated++;
                        totalRoutesUpdated += convertedRouteBatch.ConvertedRoutes.Count;
                        handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} Batches updated, {1} Routes", totalBatchesUpdated, totalRoutesUpdated);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
            return new PrepareSwitchRouteSyncRoutesForApplyOutput { };
        }

        protected override PrepareSwitchRouteSyncRoutesForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareSwitchRouteSyncRoutesForApplyInput
            {
                RangeInfo = this.RangeInfo.Get(context),
                RangeType = this.RangeType.Get(context),
                SwitchInProcess = this.SwitchInProcess.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, PrepareSwitchRouteSyncRoutesForApplyOutput result)
        {

        }

        #region Private Classes

        private class SwitchRouteSynchronizerPrepareDataForApplyContext : ISwitchRouteSynchronizerPrepareDataForApplyContext
        {
            public RouteRangeType? RouteRangeType { get; set; }

            public RouteRangeInfo RouteRangeInfo { get; set; }

            public SwitchRouteSyncInitializationData InitializationData { get; set; }

            public List<ConvertedRoute> ConvertedRoutes { get; set; }

			public string SwitchId { get; set; }
		}

        #endregion
    }
}