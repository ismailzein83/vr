using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.Invoice.Business;

namespace Vanrise.Invoice.BP.Activities
{
    #region Argument Classes
    public class InvoiceBulkActionDraftInput
    {
        public Guid InvoiceBulkActionIdentifier { get; set; }
        public BaseQueue<Entities.Invoice> OutputQueue { get; set; }
    }
    #endregion

    public sealed class InvoiceBulkActionDrafts : Vanrise.BusinessProcess.BaseAsyncActivity<InvoiceBulkActionDraftInput>
    {
        
        #region Arguments
        [RequiredArgument]
        public InArgument<Guid> InvoiceBulkActionIdentifier { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Entities.Invoice>> OutputQueue { get; set; }

        #endregion

        protected override void DoWork(InvoiceBulkActionDraftInput inputArgument, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Started Loading Invoices ...");

            InvoiceBulkActionsDraftManager invoiceBulkActionsDraftManager = new InvoiceBulkActionsDraftManager();
            PartnerManager partnerManager = new PartnerManager();
            invoiceBulkActionsDraftManager.LoadInvoicesFromInvoiceBulkActionDraft(inputArgument.InvoiceBulkActionIdentifier, (invoiceQueue) =>
            {
                inputArgument.OutputQueue.Enqueue(invoiceQueue);
                var partnerName = partnerManager.GetPartnerName(invoiceQueue.InvoiceTypeId, invoiceQueue.PartnerId);
                handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("Start execute actions for {0}.", partnerName));
            });

        }
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<Entities.Invoice>());
            base.OnBeforeExecute(context, handle);
        }
        protected override InvoiceBulkActionDraftInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new InvoiceBulkActionDraftInput()
            {
                InvoiceBulkActionIdentifier = this.InvoiceBulkActionIdentifier.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
    }
}
