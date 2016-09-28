using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.RouteSync.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

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
                        if (switchRouteSynchronizerConvertRoutesContext.InvalidRoutes != null && switchRouteSynchronizerConvertRoutesContext.InvalidRoutes.Count > 0)
                        {
                            foreach (string invalidRoute in switchRouteSynchronizerConvertRoutesContext.InvalidRoutes)
                            {
                                handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Warning, invalidRoute);
                            }
                        }
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

        #region Private Classes

        public class SwitchRouteSynchronizerConvertRoutesContext : ISwitchRouteSynchronizerConvertRoutesContext
        {
            public RouteRangeType? RouteRangeType { get; set; }

            public RouteRangeInfo RouteRangeInfo { get; set; }

            public SwitchRouteSyncInitializationData InitializationData { get; set; }

            public List<Route> Routes { get; set; }

            public List<ConvertedRoute> ConvertedRoutes { get; set; }

            public List<string> InvalidRoutes { get; set; }
        }
        #endregion
    }
}