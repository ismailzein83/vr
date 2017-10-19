using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Invoice.BP.Arguments;

namespace Vanrise.Invoice.Business
{
    class InvoiceGenerationBPDefinitionSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override void OnBPExecutionCompleted(BusinessProcess.Entities.IBPDefinitionBPExecutionCompletedContext context)
        {
            context.BPInstance.ThrowIfNull("context.BPInstance");
            InvoiceGenerationProcessInput invoiceGenerationProcessInput = context.BPInstance.InputArgument.CastWithValidate<InvoiceGenerationProcessInput>("context.IntanceToRun.InputArgument");
            new InvoiceGenerationDraftManager().ClearInvoiceGenerationDrafts(invoiceGenerationProcessInput.InvoiceGenerationIdentifier);
        }

        public override bool CanRunBPInstance(BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            InvoiceGenerationProcessInput invoiceGenerationProcessInput = context.IntanceToRun.InputArgument.CastWithValidate<InvoiceGenerationProcessInput>("context.IntanceToRun.InputArgument");

            if (invoiceGenerationProcessInput.MinimumFrom.HasValue && invoiceGenerationProcessInput.MaximumTo.HasValue)
            {
                DateTime invoiceMinimumFrom = invoiceGenerationProcessInput.MinimumFrom.Value;
                DateTime invoiceMaximumTo = invoiceGenerationProcessInput.MaximumTo.Value.AddDays(1);

                foreach (var startedBPInstance in context.GetStartedBPInstances())
                {
                    InvoiceGenerationProcessInput startedInvoiceGenerationBPArg = startedBPInstance.InputArgument as InvoiceGenerationProcessInput;
                    if (startedInvoiceGenerationBPArg != null && startedInvoiceGenerationBPArg.MinimumFrom.HasValue && startedInvoiceGenerationBPArg.MaximumTo.HasValue)
                    {
                        DateTime instanceMinimumFrom = startedInvoiceGenerationBPArg.MinimumFrom.Value;
                        DateTime instanceMaximumTo = startedInvoiceGenerationBPArg.MaximumTo.Value.AddDays(1);

                        if (Utilities.AreTimePeriodsOverlapped(invoiceMinimumFrom, invoiceMaximumTo, instanceMinimumFrom, instanceMaximumTo))
                        {
                            context.Reason = String.Format("Another invoice generation instance is running from {0:yyyy-MM-dd} to {1:yyyy-MM-dd}", startedInvoiceGenerationBPArg.MinimumFrom, startedInvoiceGenerationBPArg.MaximumTo);
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}