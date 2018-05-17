using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Queueing;

namespace TOne.WhS.RouteSync.BP.Activities
{
    #region Argument Classes

    public class ReadRoutesInput
    {
        public RouteReader RouteReader { get; set; }

        public RouteRangeType? RangeType { get; set; }

        public RouteRangeInfo RangeInfo { get; set; }

        public List<SwitchInProcess> SwitchesInProcess { get; set; }
    }

    public class ReadRoutesOutput
    {

    }

    #endregion

    public sealed class ReadRoutes : BaseAsyncActivity<ReadRoutesInput, ReadRoutesOutput>
    {
        [RequiredArgument]
        public InArgument<RouteReader> RouteReader { get; set; }

        [RequiredArgument]
        public InArgument<RouteRangeType?> RangeType { get; set; }

        [RequiredArgument]
        public InArgument<RouteRangeInfo> RangeInfo { get; set; }

        [RequiredArgument]
        public InArgument<List<SwitchInProcess>> SwitchesInProcess { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            foreach (var switchInProcess in this.SwitchesInProcess.Get(context))
            {
                if (switchInProcess.RouteQueue == null)
                    switchInProcess.RouteQueue = new MemoryQueue<RouteBatch>();
            }
            base.OnBeforeExecute(context, handle);
        }

        protected override ReadRoutesOutput DoWorkWithResult(ReadRoutesInput inputArgument, AsyncActivityHandle handle)
        {
            RouteSync.Business.ConfigManager routeSyncConfigManager = new RouteSync.Business.ConfigManager();
            int deliverRoutesBatchSize = routeSyncConfigManager.GetRouteSyncProcessRouteBatchSize();

            int totalBatchesRead = 0;
            long totalRoutesRead = 0;
            List<SwitchRouteDelivery> switchesRouteDelivery = BuildSwitchesRouteDelivery(inputArgument.SwitchesInProcess);
            Action<Route, RouteReceivedContext> onRouteReceived = BuildOnRouteReceivedAction(deliverRoutesBatchSize, switchesRouteDelivery, ref totalBatchesRead, ref totalRoutesRead, handle);
            RouteReaderContext routeReaderContext = new RouteReaderContext(() => ShouldStop(handle), onRouteReceived);
            routeReaderContext.RouteRangeInfo = inputArgument.RangeInfo;
            routeReaderContext.RouteRangeType = inputArgument.RangeType;
            inputArgument.RouteReader.ReadRoutes(routeReaderContext);

            foreach (var switchRouteDelivery in switchesRouteDelivery)
            {
                if (ShouldStop(handle))
                    break;

                if (switchRouteDelivery.PendingRoutes.Count > 0)
                {
                    totalBatchesRead++;
                    totalRoutesRead += switchRouteDelivery.PendingRoutes.Count;
                    switchRouteDelivery.SwitchInProcess.RouteQueue.Enqueue(new RouteBatch { Routes = switchRouteDelivery.PendingRoutes });
                }
            }

            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} Batches Read, {1} Routes", totalBatchesRead, totalRoutesRead);
            return new ReadRoutesOutput { };
        }

        private List<SwitchRouteDelivery> BuildSwitchesRouteDelivery(List<SwitchInProcess> switchesInProcess)
        {
            return switchesInProcess.Select(sw => new SwitchRouteDelivery
            {
                SwitchInProcess = sw,
                PendingRoutes = new List<Route>(),
                DeliverRoutesInBatches = (sw.InitializationData == null || sw.InitializationData.SupportedDeliveryMethod == RouteSyncDeliveryMethod.Batches)
            }).ToList();
        }

        private Action<Route, RouteReceivedContext> BuildOnRouteReceivedAction(int deliverRoutesBatchSize, List<SwitchRouteDelivery> switchesRouteDelivery, ref int totalBatchesRead, ref long totalRoutesRead, AsyncActivityHandle handle)
        {
            int totalBatchesRead_Internal = totalBatchesRead;
            long totalRoutesRead_Internal = totalRoutesRead;
            Action<Route, RouteReceivedContext> onRouteReceived = (route, routeReceivedContext) =>
            {
                foreach (var switchRouteDelivery in switchesRouteDelivery)
                {
                    switchRouteDelivery.PendingRoutes.Add(route);
                    if ((switchRouteDelivery.DeliverRoutesInBatches && switchRouteDelivery.PendingRoutes.Count >= deliverRoutesBatchSize))
                    {
                        switchRouteDelivery.SwitchInProcess.RouteQueue.Enqueue(new RouteBatch { Routes = switchRouteDelivery.PendingRoutes });
                        totalBatchesRead_Internal++;
                        totalRoutesRead_Internal += switchRouteDelivery.PendingRoutes.Count;
                        handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} Batches Read, {1} Routes", totalBatchesRead_Internal, totalRoutesRead_Internal);
                        switchRouteDelivery.PendingRoutes = new List<Route>();
                    }
                }
            };
            totalBatchesRead = totalBatchesRead_Internal;
            totalRoutesRead = totalRoutesRead_Internal;
            return onRouteReceived;
        }

        protected override ReadRoutesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ReadRoutesInput()
            {
                RangeInfo = this.RangeInfo.Get(context),
                RangeType = this.RangeType.Get(context),
                RouteReader = this.RouteReader.Get(context),
                SwitchesInProcess = this.SwitchesInProcess.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ReadRoutesOutput result)
        {

        }

        #region Private Classes

        private class RouteReaderContext : IRouteReaderContext
        {
            public RouteRangeType? RouteRangeType { get; set; }
            public RouteRangeInfo RouteRangeInfo { get; set; }
            
            Action<Route, RouteReceivedContext> _onRouteReceived;
            Func<bool> _shouldStop;

            public RouteReaderContext(Func<bool> shouldStop, Action<Route, RouteReceivedContext> onRouteReceived)
            {
                _shouldStop = shouldStop;
                _onRouteReceived = onRouteReceived;
            }

            public bool ShouldStop()
            {
                return _shouldStop();
            }
            
            public void OnRouteReceived(Route route, RouteReceivedContext context)
            {
                _onRouteReceived(route, context);
            }
        }

        private class SwitchRouteDelivery
        {
            public SwitchInProcess SwitchInProcess { get; set; }

            public List<Route> PendingRoutes { get; set; }

            public bool DeliverRoutesInBatches { get; set; }
        }

        #endregion
    }
}
