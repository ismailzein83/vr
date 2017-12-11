using System.Collections.Generic;
using System.Activities;
using TOne.WhS.RouteSync.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.BP.Activities
{
    #region Argument Classes

    public class ConvertSwitchRouteSyncRoutesInput
    {
        public RouteRangeType? RangeType { get; set; }

        public RouteRangeInfo RangeInfo { get; set; }

        public SwitchInProcess SwitchInProcess { get; set; }
    }

    public class ConvertSwitchRouteSyncRoutesOutput
    {

    }

    #endregion

    public sealed class ConvertSwitchRouteSyncRoutes : DependentAsyncActivity<ConvertSwitchRouteSyncRoutesInput, ConvertSwitchRouteSyncRoutesOutput>
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
            if (switcheInProcess.ConvertedRouteQueue == null)
                switcheInProcess.ConvertedRouteQueue = new MemoryQueue<ConvertedRouteBatch>();
            base.OnBeforeExecute(context, handle);
        }

        protected override ConvertSwitchRouteSyncRoutesOutput DoWorkWithResult(ConvertSwitchRouteSyncRoutesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            int totalBatchesConverted = 0;
            long totalRoutesConverted = 0;
            var switchInProcess = inputArgument.SwitchInProcess;

            inputArgument.SwitchInProcess.ThrowIfNull("inputArgument.SwitchInProcess");
            inputArgument.SwitchInProcess.RouteQueue.ThrowIfNull("inputArgument.SwitchInProcess.RouteQueue");
            switchInProcess.Switch.ThrowIfNull("switchInProcess.Switch");
            switchInProcess.Switch.RouteSynchronizer.ThrowIfNull("switchInProcess.Switch.RouteSynchronizer");

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.SwitchInProcess.RouteQueue.TryDequeue((routeBatch) =>
                    {
                        SwitchRouteSynchronizerConvertRoutesContext switchRouteSynchronizerConvertRoutesContext = new SwitchRouteSynchronizerConvertRoutesContext
                        {
                            Routes = routeBatch.Routes,
                            RouteRangeType = inputArgument.RangeType,
                            RouteRangeInfo = inputArgument.RangeInfo,
                            InitializationData = switchInProcess.InitializationData
                        };
                        switchInProcess.Switch.RouteSynchronizer.ConvertRoutes(switchRouteSynchronizerConvertRoutesContext);

                        if (switchRouteSynchronizerConvertRoutesContext.ConvertedRoutes != null && switchRouteSynchronizerConvertRoutesContext.ConvertedRoutes.Count > 0)
                        {
                            totalBatchesConverted++;
                            totalRoutesConverted += switchRouteSynchronizerConvertRoutesContext.ConvertedRoutes.Count;
                            switchInProcess.ConvertedRouteQueue.Enqueue(new ConvertedRouteBatch
                                {
                                    ConvertedRoutes = switchRouteSynchronizerConvertRoutesContext.ConvertedRoutes
                                });
                            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} Batches converted, {1} Routes", totalBatchesConverted, totalRoutesConverted);
                        }
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
            return new ConvertSwitchRouteSyncRoutesOutput { };
        }

        protected override ConvertSwitchRouteSyncRoutesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ConvertSwitchRouteSyncRoutesInput
            {
                RangeInfo = this.RangeInfo.Get(context),
                RangeType = this.RangeType.Get(context),
                SwitchInProcess = this.SwitchInProcess.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ConvertSwitchRouteSyncRoutesOutput result)
        {

        }
    }
}