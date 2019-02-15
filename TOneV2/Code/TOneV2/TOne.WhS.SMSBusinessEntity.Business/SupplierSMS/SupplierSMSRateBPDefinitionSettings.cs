using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SMSBusinessEntity.BP.Arguments;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace TOne.WhS.SMSBusinessEntity.Business
{
    public class SupplierSMSRateBPDefinitionSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(Vanrise.BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            SMSSupplierRateInput inputArg = context.IntanceToRun.InputArgument.CastWithValidate<SMSSupplierRateInput>("context.IntanceToRun.InputArgument");
            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                SMSSupplierRateInput startedBPInstanceInputArg = startedBPInstance.InputArgument as SMSSupplierRateInput;
                if (startedBPInstanceInputArg != null)
                {
                    if (startedBPInstanceInputArg.SupplierID == inputArg.SupplierID)
                    {
                        string supplierName = new CarrierAccountManager().GetCarrierAccountName(inputArg.SupplierID);
                        context.Reason = $"Same process for Supplier {supplierName} is running";
                        return false;
                    }
                }
            }

            return true;
        }

        public override void OnBPExecutionCompleted(Vanrise.BusinessProcess.Entities.IBPDefinitionBPExecutionCompletedContext context)
        {

            if (context.BPInstance.Status == BPInstanceStatus.Completed)
            {
                SMSSupplierRateInput inputArg = context.BPInstance.InputArgument.CastWithValidate<SMSSupplierRateInput>("context.BPInstance.InputArgument");

                int userID = context.BPInstance.InitiatorUserId;
                UpdateSupplierSMSDraftStatusInput input = new UpdateSupplierSMSDraftStatusInput() { NewStatus = ProcessStatus.Completed, ProcessDraftID = inputArg.ProcessDraftID };
                new SupplierSMSRateDraftManager().UpdateSMSRateChangesStatus(input, userID);
            }
        }
    }
}