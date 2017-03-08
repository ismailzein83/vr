using System.Activities;
using Vanrise.BusinessProcess;
using TOne.WhS.RouteSync.Entities;
using System;
using TOne.WhS.RouteSync.Business;

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
            ConfigManager configManager = new ConfigManager();
            int indexesCommandTimeoutInSeconds = configManager.GetRouteSyncProcessIndexesCommandTimeoutInSeconds();

            var switchRouteSynchronizerFinalizeContext = new SwitchRouteSynchronizerFinalizeContext(handle) { RouteRangeType = inputArgument.RouteRangeType, InitializationData = inputArgument.InitializationData, SwitchName = inputArgument.Switch.Name, IndexesCommandTimeoutInSeconds = indexesCommandTimeoutInSeconds };
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
            AsyncActivityHandle _handle;

            public SwitchRouteSynchronizerFinalizeContext(AsyncActivityHandle handle)
            {
                if (handle == null)
                    throw new ArgumentNullException("handle");
                _handle = handle;
            }
            public string SwitchName { get; set; }

            public RouteRangeType? RouteRangeType { get; set; }

            public SwitchRouteSyncInitializationData InitializationData { get; set; }

            public int IndexesCommandTimeoutInSeconds { get; set; }

            public void WriteTrackingMessage(Vanrise.Entities.LogEntryType severity, string messageFormat, params object[] args)
            {
                _handle.SharedInstanceData.WriteBusinessTrackingMsg(severity, messageFormat, args);
            }
        }
        #endregion
    }
}
