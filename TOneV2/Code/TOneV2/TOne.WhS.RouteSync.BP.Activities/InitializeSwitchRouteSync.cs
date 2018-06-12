using System;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.BP.Activities
{
    public sealed class InitializeSwitchRouteSync : CodeActivity
    {
        [RequiredArgument]
        public InArgument<SwitchInfo> Switch { get; set; }

        [RequiredArgument]
        public InArgument<RouteRangeType?> RouteRangeType { get; set; }

        [RequiredArgument]
        public OutArgument<SwitchRouteSyncInitializationData> InitializationData { get; set; }

        [RequiredArgument]
        public OutArgument<SwitchSyncOutput> SwitchSyncOutput { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            SwitchInfo switchInfo = this.Switch.Get(context);
            RouteRangeType? routeRangeType = this.RouteRangeType.Get(context);

            SwitchSyncOutput switchSyncOutput;
            var switchRouteSynchronizerInitializeContext = new SwitchRouteSynchronizerInitializeContext
            {
                SwitchId = switchInfo.SwitchId,
                SwitchName = switchInfo.Name,
                RouteRangeType = routeRangeType,
                WriteBusinessHandledException = context.GetSharedInstanceData().WriteBusinessHandledException
            };

            try
            {
                switchInfo.RouteSynchronizer.Initialize(switchRouteSynchronizerInitializeContext);
                switchSyncOutput = switchRouteSynchronizerInitializeContext.SwitchSyncOutput;
                this.InitializationData.Set(context, switchRouteSynchronizerInitializeContext.InitializationData);
            }
            catch (Exception ex)
            {
                string errorBusinessMessage = Utilities.GetExceptionBusinessMessage(ex);
                string exceptionDetail = ex.ToString();
                SwitchRouteSynchroniserOutput output = new SwitchRouteSynchroniserOutput() { ErrorBusinessMessage = errorBusinessMessage, ExceptionDetail = exceptionDetail };
                switchSyncOutput = new SwitchSyncOutput() { SwitchId = switchInfo.SwitchId, SwitchSyncResult = SwitchSyncResult.Failed, SwitchRouteSynchroniserOutputList = new List<SwitchRouteSynchroniserOutput>() { output } };
                VRBusinessException exception = new VRBusinessException(string.Format("Error occured while initializing Switch '{0}'", switchInfo.Name), ex);
                context.GetSharedInstanceData().WriteBusinessHandledException(exception);
            }

            this.SwitchSyncOutput.Set(context, switchSyncOutput);
        }

        #region Private Classes

        private class SwitchRouteSynchronizerInitializeContext : ISwitchRouteSynchronizerInitializeContext
        {
            public string SwitchId { get; set; }

            public string SwitchName { get; set; }

            public RouteRangeType? RouteRangeType { get; set; }

            public Action<Exception, bool> WriteBusinessHandledException { get; set; }

            public SwitchSyncOutput SwitchSyncOutput { get; set; }

            public SwitchRouteSyncInitializationData InitializationData { get; set; }
        }

        #endregion
    }
}