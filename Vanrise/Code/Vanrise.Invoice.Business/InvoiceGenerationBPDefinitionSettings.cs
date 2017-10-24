using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Invoice.BP.Arguments;
using Vanrise.Invoice.Entities;
using Vanrise.Queueing;
using Vanrise.Queueing.Entities;

namespace Vanrise.Invoice.Business
{
    class InvoiceGenerationBPDefinitionSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override void OnBPExecutionCompleted(BusinessProcess.Entities.IBPDefinitionBPExecutionCompletedContext context)
        {
            context.BPInstance.ThrowIfNull("context.BPInstance");
            new HoldRequestManager().DeleteHoldRequestByBPInstanceId(context.BPInstance.ProcessInstanceID);

            InvoiceGenerationProcessInput invoiceGenerationProcessInput = context.BPInstance.InputArgument.CastWithValidate<InvoiceGenerationProcessInput>("context.IntanceToRun.InputArgument");
            new InvoiceGenerationDraftManager().ClearInvoiceGenerationDrafts(invoiceGenerationProcessInput.InvoiceGenerationIdentifier);
        }

        public override bool CanRunBPInstance(BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            InvoiceGenerationProcessInput invoiceGenerationProcessInput = context.IntanceToRun.InputArgument.CastWithValidate<InvoiceGenerationProcessInput>("context.IntanceToRun.InputArgument");

            DateTime invoiceMinimumFrom = invoiceGenerationProcessInput.MinimumFrom;
            DateTime invoiceMaximumTo = invoiceGenerationProcessInput.MaximumTo.AddDays(1);

            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                InvoiceGenerationProcessInput startedInvoiceGenerationBPArg = startedBPInstance.InputArgument as InvoiceGenerationProcessInput;
                if (startedInvoiceGenerationBPArg != null)
                {
                    DateTime instanceMinimumFrom = startedInvoiceGenerationBPArg.MinimumFrom;
                    DateTime instanceMaximumTo = startedInvoiceGenerationBPArg.MaximumTo.AddDays(1);

                    if (Utilities.AreTimePeriodsOverlapped(invoiceMinimumFrom, invoiceMaximumTo, instanceMinimumFrom, instanceMaximumTo))
                    {
                        context.Reason = String.Format("Another invoice generation instance is running from {0:yyyy-MM-dd} to {1:yyyy-MM-dd}", startedInvoiceGenerationBPArg.MinimumFrom, startedInvoiceGenerationBPArg.MaximumTo);
                        return false;
                    }
                }
            }

            HoldRequestManager holdRequestManager = new HoldRequestManager();
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            InvoiceType invoiceType = invoiceTypeManager.GetInvoiceType(invoiceGenerationProcessInput.InvoiceTypeId);
            invoiceType.ThrowIfNull("invoiceType", invoiceGenerationProcessInput.InvoiceTypeId);
            invoiceType.Settings.ThrowIfNull("invoiceType.Settings", invoiceGenerationProcessInput.InvoiceTypeId);

            HoldRequest existingHoldRequest = holdRequestManager.GetHoldRequest(context.IntanceToRun.ProcessInstanceID);
            if (existingHoldRequest == null)
            {
                holdRequestManager.InsertHoldRequest(context.IntanceToRun.ProcessInstanceID, invoiceType.Settings.ExecutionFlowDefinitionId, invoiceMinimumFrom, invoiceMaximumTo,
                  invoiceType.Settings.StagesToHoldNames, invoiceType.Settings.StagesToProcessNames, HoldRequestStatus.Pending);

                context.Reason = "Waiting CDR Import";
                return false;
            }
            else if (existingHoldRequest.Status != HoldRequestStatus.CanBeStarted)
            {
                context.Reason = "Waiting CDR Import";
                return false;
            }

            return true;
        }
    }
}