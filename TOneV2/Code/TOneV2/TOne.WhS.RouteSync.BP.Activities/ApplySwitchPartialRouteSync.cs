using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.BP.Activities
{
    public class ApplySwitchPartialRouteSyncInput
    {
        public SwitchInfo SwitchInfo { get; set; }

        public Dictionary<string, List<CustomerRoute>> CustomerRoutesBySwitchId { get; set; }
    }

    public class ApplySwitchPartialRouteSyncOutput
    {
        public SwitchSyncOutput SwitchSyncOutput { get; set; }
    }

    public sealed class ApplySwitchPartialRouteSync : BaseAsyncActivity<ApplySwitchPartialRouteSyncInput, ApplySwitchPartialRouteSyncOutput>
    {
        [RequiredArgument]
        public InArgument<SwitchInfo> SwitchInfo { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<string, List<CustomerRoute>>> CustomerRoutesBySwitchId { get; set; }

        [RequiredArgument]
        public OutArgument<SwitchSyncOutput> SwitchSyncOutput { get; set; }


        protected override ApplySwitchPartialRouteSyncOutput DoWorkWithResult(ApplySwitchPartialRouteSyncInput inputArgument, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("Partial Route Sync for Switch: '{0}' has started", inputArgument.SwitchInfo.Name));

            SwitchSyncOutput switchSyncOutput = null;
            List<CustomerRoute> updatedCustomerRoutes;
            if (inputArgument.CustomerRoutesBySwitchId.TryGetValue(inputArgument.SwitchInfo.SwitchId, out updatedCustomerRoutes))
            {
                var applyDifferentialRoutesContext = new SwitchRouteSynchronizerApplyDifferentialRoutesContext()
                {
                    SwitchId = inputArgument.SwitchInfo.SwitchId,
                    SwitchName = inputArgument.SwitchInfo.Name,
                    UpdatedRoutes = Helper.BuildRoutesFromCustomerRoutes(updatedCustomerRoutes),
                    WriteBusinessHandledException = handle.SharedInstanceData.WriteBusinessHandledException
                };

                try
                {
                    inputArgument.SwitchInfo.RouteSynchronizer.ApplyDifferentialRoutes(applyDifferentialRoutesContext);
                    switchSyncOutput = applyDifferentialRoutesContext.SwitchSyncOutput;
                }
                catch (Exception ex)
                {
                    string errorBusinessMessage = Utilities.GetExceptionBusinessMessage(ex);
                    string exceptionDetail = ex.ToString();
                    SwitchRouteSynchroniserOutput output = new SwitchRouteSynchroniserOutput() { ErrorBusinessMessage = errorBusinessMessage, ExceptionDetail = exceptionDetail };
                    switchSyncOutput = new SwitchSyncOutput() { SwitchId = inputArgument.SwitchInfo.SwitchId, SwitchSyncResult = SwitchSyncResult.Failed, SwitchRouteSynchroniserOutputList = new List<SwitchRouteSynchroniserOutput>() { output } };
                    VRBusinessException exception = new VRBusinessException(string.Format("Error occured while applying partial route sync for Switch '{0}'", inputArgument.SwitchInfo.Name), ex);
                    handle.SharedInstanceData.WriteBusinessHandledException(exception);
                }
            }

            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("Partial Route Sync for Switch: '{0}' has finished", inputArgument.SwitchInfo.Name));

            return new ApplySwitchPartialRouteSyncOutput { SwitchSyncOutput = switchSyncOutput };
        }

        protected override ApplySwitchPartialRouteSyncInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplySwitchPartialRouteSyncInput()
            {
                SwitchInfo = this.SwitchInfo.Get(context),
                CustomerRoutesBySwitchId = this.CustomerRoutesBySwitchId.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplySwitchPartialRouteSyncOutput result)
        {
            this.SwitchSyncOutput.Set(context, result.SwitchSyncOutput);
        }
    }
}