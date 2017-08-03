using System;
using System.Activities;
using System.Collections.Concurrent;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Business;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.BP.Activities
{
    #region Argument Classes

    public class FinalizeSwitchRouteSyncInput
    {
        public SwitchInfo Switch { get; set; }

        public RouteRangeType? RouteRangeType { get; set; }

        public ConcurrentDictionary<string, SwitchSyncOutput> SwitchSyncOutputDict { get; set; }

        public SwitchRouteSyncInitializationData InitializationData { get; set; }

    }

    public class FinalizeSwitchRouteSyncOutput
    {
        public SwitchSyncOutput SwitchSyncOutput { get; set; }
    }

    #endregion

    public sealed class FinalizeSwitchRouteSync : BaseAsyncActivity<FinalizeSwitchRouteSyncInput, FinalizeSwitchRouteSyncOutput>
    {
        [RequiredArgument]
        public InArgument<SwitchInfo> Switch { get; set; }

        [RequiredArgument]
        public InArgument<RouteRangeType?> RouteRangeType { get; set; }

        [RequiredArgument]
        public InArgument<ConcurrentDictionary<string, SwitchSyncOutput>> SwitchSyncOutputDict { get; set; }

        [RequiredArgument]
        public InArgument<SwitchRouteSyncInitializationData> InitializationData { get; set; }

        [RequiredArgument]
        public OutArgument<SwitchSyncOutput> SwitchSyncOutput { get; set; }

        protected override FinalizeSwitchRouteSyncOutput DoWorkWithResult(FinalizeSwitchRouteSyncInput inputArgument, AsyncActivityHandle handle)
        {
            SwitchSyncOutput switchSyncOutput = null;
            SwitchSyncOutput previousSwitchSyncOutput = null;
            if (inputArgument.SwitchSyncOutputDict != null)
                previousSwitchSyncOutput = inputArgument.SwitchSyncOutputDict.GetRecord(inputArgument.Switch.SwitchId);

            if (previousSwitchSyncOutput == null || previousSwitchSyncOutput.SwitchSyncResult != SwitchSyncResult.Failed)
            {
                try
                {
                    ConfigManager configManager = new ConfigManager();
                    int indexesCommandTimeoutInSeconds = configManager.GetRouteSyncProcessIndexesCommandTimeoutInSeconds();

                    var switchRouteSynchronizerFinalizeContext = new SwitchRouteSynchronizerFinalizeContext(inputArgument.RouteRangeType, inputArgument.InitializationData, handle.SharedInstanceData.WriteTrackingMessage, inputArgument.Switch.Name, indexesCommandTimeoutInSeconds, inputArgument.Switch.SwitchId, previousSwitchSyncOutput, handle.SharedInstanceData.WriteBusinessHandledException);
                    inputArgument.Switch.RouteSynchronizer.Finalize(switchRouteSynchronizerFinalizeContext);
                    switchSyncOutput = switchRouteSynchronizerFinalizeContext.SwitchSyncOutput;
                }
                catch (Exception ex)
                {
                    string errorBusinessMessage = Utilities.GetExceptionBusinessMessage(ex);
                    string exceptionDetail = ex.ToString();
                    SwitchRouteSynchroniserOutput output = new SwitchRouteSynchroniserOutput() { ErrorBusinessMessage = errorBusinessMessage, ExceptionDetail = exceptionDetail };
                    switchSyncOutput = new SwitchSyncOutput() { SwitchId = inputArgument.Switch.SwitchId, SwitchSyncResult = SwitchSyncResult.Failed, SwitchRouteSynchroniserOutputList = new System.Collections.Generic.List<SwitchRouteSynchroniserOutput>() { output } };
                    VRBusinessException exception = new VRBusinessException(string.Format("Error occured while finalizing Switch '{0}'", inputArgument.Switch.Name), ex);
                    handle.SharedInstanceData.WriteBusinessHandledException(exception);
                }
            }
            return new FinalizeSwitchRouteSyncOutput() { SwitchSyncOutput = switchSyncOutput };
        }

        protected override FinalizeSwitchRouteSyncInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new FinalizeSwitchRouteSyncInput()
            {
                Switch = this.Switch.Get(context),
                RouteRangeType = this.RouteRangeType.Get(context),
                SwitchSyncOutputDict = this.SwitchSyncOutputDict.Get(context),
                InitializationData = this.InitializationData.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, FinalizeSwitchRouteSyncOutput result)
        {
            this.SwitchSyncOutput.Set(context, result.SwitchSyncOutput);
        }

        #region Private Classes

        private class SwitchRouteSynchronizerFinalizeContext : ISwitchRouteSynchronizerFinalizeContext
        {
            public SwitchRouteSynchronizerFinalizeContext(RouteRangeType? routeRangeType, SwitchRouteSyncInitializationData initializationData, Action<LogEntryType, string, object[]> writeTrackingMessage,
                string switchName, int indexesCommandTimeoutInSeconds, string switchId, SwitchSyncOutput previousSwitchSyncOutput, Action<Exception, bool> writeBusinessHandledException)
            {
                RouteRangeType = routeRangeType;
                InitializationData = initializationData;
                WriteTrackingMessage = writeTrackingMessage;
                SwitchName = switchName;
                IndexesCommandTimeoutInSeconds = indexesCommandTimeoutInSeconds;
                SwitchId = switchId;
                PreviousSwitchSyncOutput = previousSwitchSyncOutput;
                WriteBusinessHandledException = writeBusinessHandledException;
            }

            public string SwitchName { get; set; }

            public RouteRangeType? RouteRangeType { get; set; }

            public SwitchRouteSyncInitializationData InitializationData { get; set; }

            public int IndexesCommandTimeoutInSeconds { get; set; }

            public string SwitchId { get; set; }

            public SwitchSyncOutput SwitchSyncOutput { get; set; }

            public Action<LogEntryType, string, object[]> WriteTrackingMessage { get; set; }

            public SwitchSyncOutput PreviousSwitchSyncOutput { get; set; }

            public Action<Exception, bool> WriteBusinessHandledException { get; set; }
        }
        #endregion
    }
}
