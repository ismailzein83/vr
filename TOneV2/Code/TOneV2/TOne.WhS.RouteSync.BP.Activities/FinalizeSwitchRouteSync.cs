using System.Activities;
using Vanrise.BusinessProcess;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.BP.Activities
{
    #region Argument Classes

    public class FinalizeSwitchRouteSyncInput
    {
        public SwitchInfo Switch { get; set; }

        public RouteRangeType? RouteRangeType { get; set; }

        public SwitchRouteSyncInitializationData InitializationData { get; set; }

    }

    public class FinalizeSwitchRouteSyncOutput
    {

    }

    #endregion

    public sealed class FinalizeSwitchRouteSync : BaseAsyncActivity<FinalizeSwitchRouteSyncInput, FinalizeSwitchRouteSyncOutput>
    {
        [RequiredArgument]
        public InArgument<SwitchInfo> Switch { get; set; }

        [RequiredArgument]
        public InArgument<RouteRangeType?> RouteRangeType { get; set; }

        [RequiredArgument]
        public InArgument<SwitchRouteSyncInitializationData> InitializationData { get; set; }

        protected override FinalizeSwitchRouteSyncOutput DoWorkWithResult(FinalizeSwitchRouteSyncInput inputArgument, AsyncActivityHandle handle)
        {
            var switchRouteSynchronizerFinalizeContext = new SwitchRouteSynchronizerFinalizeContext { RouteRangeType = inputArgument.RouteRangeType, InitializationData = inputArgument.InitializationData };
            inputArgument.Switch.RouteSynchronizer.Finalize(switchRouteSynchronizerFinalizeContext);
            return new FinalizeSwitchRouteSyncOutput();
        }


        protected override FinalizeSwitchRouteSyncInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new FinalizeSwitchRouteSyncInput()
            {
                Switch = this.Switch.Get(context),
                RouteRangeType = this.RouteRangeType.Get(context),
                InitializationData = this.InitializationData.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, FinalizeSwitchRouteSyncOutput result)
        {

        }

        #region Private Classes

        private class SwitchRouteSynchronizerFinalizeContext : ISwitchRouteSynchronizerFinalizeContext
        {
            public RouteRangeType? RouteRangeType
            {
                get;
                set;
            }

            public SwitchRouteSyncInitializationData InitializationData
            {
                get;
                set;
            }
        }
        #endregion
    }
}
