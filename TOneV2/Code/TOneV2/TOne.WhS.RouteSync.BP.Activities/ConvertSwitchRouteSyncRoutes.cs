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
            var switchInProcess = this.SwitchInProcess.Get(context);
            if (switchInProcess.ConvertedRouteQueue == null)
                switchInProcess.ConvertedRouteQueue = new MemoryQueue<ConvertedRouteBatch>();
            base.OnBeforeExecute(context, handle);
        }

        protected override ConvertSwitchRouteSyncRoutesOutput DoWorkWithResult(ConvertSwitchRouteSyncRoutesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            SwitchInProcess switchInProcess = inputArgument.SwitchInProcess;
            switchInProcess.ThrowIfNull("inputArgument.SwitchInProcess");
            switchInProcess.RouteQueue.ThrowIfNull("inputArgument.SwitchInProcess.RouteQueue");
            switchInProcess.Switch.ThrowIfNull("switchInProcess.Switch");
            switchInProcess.Switch.RouteSynchronizer.ThrowIfNull("switchInProcess.Switch.RouteSynchronizer");

            int totalBatchesConverted = 0;
            long totalRoutesConverted = 0;

            object convertedRoutesPayload = null;

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.SwitchInProcess.RouteQueue.TryDequeue((routeBatch) =>
                    {
                        var switchRouteSynchronizerConvertRoutesContext = new SwitchRouteSynchronizerConvertRoutesContext
                        {
                            Routes = routeBatch.Routes,
                            RouteRangeType = inputArgument.RangeType,
                            RouteRangeInfo = inputArgument.RangeInfo,
                            InitializationData = switchInProcess.InitializationData,
                            SwitchId = inputArgument.SwitchInProcess.Switch.SwitchId,
                            ConvertedRoutesPayload = convertedRoutesPayload
                        };
                        switchInProcess.Switch.RouteSynchronizer.ConvertRoutes(switchRouteSynchronizerConvertRoutesContext);
                        convertedRoutesPayload = switchRouteSynchronizerConvertRoutesContext.ConvertedRoutesPayload;

                        if (switchRouteSynchronizerConvertRoutesContext.ConvertedRoutes != null && switchRouteSynchronizerConvertRoutesContext.ConvertedRoutes.Count > 0)
                        {
                            totalBatchesConverted++;
                            totalRoutesConverted += switchRouteSynchronizerConvertRoutesContext.ConvertedRoutes.Count;
                            switchInProcess.ConvertedRouteQueue.Enqueue(new ConvertedRouteBatch { ConvertedRoutes = switchRouteSynchronizerConvertRoutesContext.ConvertedRoutes });

                            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} Batches converted, {1} Routes", totalBatchesConverted, totalRoutesConverted);
                        }
                    });
                } while (!ShouldStop(handle) && hasItem);
            });

            var switchRouteSynchronizerOnAllRoutesConvertedContext = new SwitchRouteSynchronizerOnAllRoutesConvertedContext() { ConvertedRoutesPayload = convertedRoutesPayload };
            switchInProcess.Switch.RouteSynchronizer.onAllRoutesConverted(switchRouteSynchronizerOnAllRoutesConvertedContext);
            if (switchRouteSynchronizerOnAllRoutesConvertedContext.ConvertedRoutes != null && switchRouteSynchronizerOnAllRoutesConvertedContext.ConvertedRoutes.Count > 0)
            {
                totalRoutesConverted += switchRouteSynchronizerOnAllRoutesConvertedContext.ConvertedRoutes.Count;
                switchInProcess.ConvertedRouteQueue.Enqueue(new ConvertedRouteBatch { ConvertedRoutes = switchRouteSynchronizerOnAllRoutesConvertedContext.ConvertedRoutes });

                handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "1 Batches converted, {0} Routes", totalRoutesConverted);
            }

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