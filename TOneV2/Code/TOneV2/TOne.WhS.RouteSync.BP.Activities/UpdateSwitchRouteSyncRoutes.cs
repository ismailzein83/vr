using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.RouteSync.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.RouteSync.BP.Activities
{
    #region Argument Classes

    public class UpdateSwitchRouteSyncRoutesInput
    {
        public RouteRangeType? RangeType { get; set; }

        public RouteRangeInfo RangeInfo { get; set; }

        public SwitchInProcess SwitchInProcess { get; set; }
    }

    public class UpdateSwitchRouteSyncRoutesOutput
    {

    }

    #endregion

    public sealed class UpdateSwitchRouteSyncRoutes : DependentAsyncActivity<UpdateSwitchRouteSyncRoutesInput, UpdateSwitchRouteSyncRoutesOutput>
    {
        [RequiredArgument]
        public InArgument<RouteRangeType?> RangeType { get; set; }

        [RequiredArgument]
        public InArgument<RouteRangeInfo> RangeInfo { get; set; }

        [RequiredArgument]
        public InArgument<SwitchInProcess> SwitchInProcess { get; set; }


        protected override UpdateSwitchRouteSyncRoutesOutput DoWorkWithResult(UpdateSwitchRouteSyncRoutesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            var switchInProcess = inputArgument.SwitchInProcess;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.SwitchInProcess.ConvertedRouteQueue.TryDequeue((convertedRouteBatch) =>
                    {
                        SwitchRouteSynchronizerUpdateConvertedRoutesContext switchRouteSynchronizerUpdateConvertedRoutesContext = new SwitchRouteSynchronizerUpdateConvertedRoutesContext
                        {
                            ConvertedRoutes = convertedRouteBatch.ConvertedRoutes,
                            RouteRangeType = inputArgument.RangeType,
                            RouteRangeInfo = inputArgument.RangeInfo,
                            InitializationData = switchInProcess.InitializationData
                        };
                        switchInProcess.Switch.RouteSynchronizer.UpdateConvertedRoutes(switchRouteSynchronizerUpdateConvertedRoutesContext);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
            return new UpdateSwitchRouteSyncRoutesOutput { };
        }

        protected override UpdateSwitchRouteSyncRoutesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new UpdateSwitchRouteSyncRoutesInput
            {
                RangeInfo = this.RangeInfo.Get(context),
                RangeType = this.RangeType.Get(context),
                SwitchInProcess = this.SwitchInProcess.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, UpdateSwitchRouteSyncRoutesOutput result)
        {

        }

        #region Private Classes

        private class SwitchRouteSynchronizerUpdateConvertedRoutesContext : ISwitchRouteSynchronizerUpdateConvertedRoutesContext
        {
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

            public SwitchRouteSyncInitializationData InitializationData
            {
                get;
                set;
            }

            public List<ConvertedRoute> ConvertedRoutes
            {
                get;
                set;
            }
        }


        #endregion
    }

}
