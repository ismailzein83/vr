using System;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SMSBusinessEntity.BP.Arguments;
using TOne.WhS.SMSBusinessEntity.Data;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace TOne.WhS.SMSBusinessEntity.Business
{
    public class CustomerSMSRateBPDefinitionSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(Vanrise.BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            SMSSaleRateInput inputArg = context.IntanceToRun.InputArgument.CastWithValidate<SMSSaleRateInput>("context.IntanceToRun.InputArgument");
            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                SMSSaleRateInput startedBPInstanceInputArg = startedBPInstance.InputArgument as SMSSaleRateInput;
                if (startedBPInstanceInputArg != null)
                {
                    if (startedBPInstanceInputArg.CustomerID == inputArg.CustomerID)
                    {
                        string customerName = new CarrierAccountManager().GetCarrierAccountName(inputArg.CustomerID);
                        context.Reason = $"Same process for Customer {customerName} is running";
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
                SMSSaleRateInput inputArg = context.BPInstance.InputArgument.CastWithValidate<SMSSaleRateInput>("context.BPInstance.InputArgument");

                int userID = context.BPInstance.InitiatorUserId;
                UpdateCustomerSMSDraftStatusInput input = new UpdateCustomerSMSDraftStatusInput() { NewStatus = ProcessStatus.Completed, ProcessDraftID = inputArg.ProcessDraftID };
                new CustomerSMSRateDraftManager().UpdateSMSRateChangesStatus(input, userID);
            }
        }
    }
}