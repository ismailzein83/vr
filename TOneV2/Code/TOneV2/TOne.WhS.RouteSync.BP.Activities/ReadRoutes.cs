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
            foreach(var switchInProcess in this.SwitchesInProcess.Get(context))
            {
                if (switchInProcess.RouteQueue == null)
                    switchInProcess.RouteQueue = new MemoryQueue<RouteBatch>();
            }
            base.OnBeforeExecute(context, handle);
        }

        protected override ReadRoutesOutput DoWorkWithResult(ReadRoutesInput inputArgument, AsyncActivityHandle handle)
        {
            int deliverRoutesBatchSize = 1000;
            List<SwitchRouteDelivery> switchesRouteDelivery = inputArgument.SwitchesInProcess.Select(sw => new SwitchRouteDelivery
            {
                SwitchInProcess = sw,
                PendingRoutes = new List<Route>(),
                 DeliverRoutesInBatches = (sw.InitializationData == null || sw.InitializationData.SupportedDeliveryMethod == RouteSyncDeliveryMethod.Batches)
            }).ToList();
            Action<Route, RouteReceivedContext> onRouteReceived = (route, routeReceivedContext) =>
                {
                    foreach (var switchRouteDelivery in switchesRouteDelivery)
                    {
                        switchRouteDelivery.PendingRoutes.Add(route);
                        if (routeReceivedContext.IsLastRoute || (switchRouteDelivery.PendingRoutes.Count >= deliverRoutesBatchSize && switchRouteDelivery.DeliverRoutesInBatches))
                        {
                            switchRouteDelivery.SwitchInProcess.RouteQueue.Enqueue(new RouteBatch { Routes = switchRouteDelivery.PendingRoutes });
                            switchRouteDelivery.PendingRoutes = new List<Route>();
                        }
                    }
                };
            RouteReaderContext routeReaderContext = new RouteReaderContext(onRouteReceived);
            routeReaderContext.RouteRangeInfo = inputArgument.RangeInfo;
            routeReaderContext.RouteRangeType = inputArgument.RangeType;
            inputArgument.RouteReader.ReadRoutes(routeReaderContext);
            return new ReadRoutesOutput { };
        }

        protected override ReadRoutesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            throw new NotImplementedException();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ReadRoutesOutput result)
        {
            throw new NotImplementedException();
        }

        #region Private Classes

        private class RouteReaderContext : IRouteReaderContext
        {
            Action<Route, RouteReceivedContext> _onRouteReceived;
            public RouteReaderContext(Action<Route, RouteReceivedContext> onRouteReceived)
            {
                _onRouteReceived = onRouteReceived;
            }
            public RouteRangeType? RouteRangeType
            {
                get;
                set;
            }

            public RouteRangeInfo RouteRangeInfo
            {
                get;
                set;
            }

            public void OnRouteReceived(Route route, RouteReceivedContext context)
            {
                OnRouteReceived(route, context);
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
