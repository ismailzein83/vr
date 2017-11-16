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
    public class LoadInvoiceBulkActionDraftInput
    {
        public Guid InvoiceBulkActionIdentifier { get; set; }
        public BaseQueue<Entities.Invoice> OutputQueue { get; set; }
    }
    #endregion

    public sealed class LoadInvoiceBulkActionDrafts : Vanrise.BusinessProcess.BaseAsyncActivity<LoadInvoiceBulkActionDraftInput>
    {
        
        #region Arguments
        [RequiredArgument]
        public InArgument<Guid> InvoiceBulkActionIdentifier { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Entities.Invoice>> OutputQueue { get; set; }

        #endregion

        protected override void DoWork(LoadInvoiceBulkActionDraftInput inputArgument, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Started Loading Invoices ...");

            InvoiceBulkActionsDraftManager invoiceBulkActionsDraftManager = new InvoiceBulkActionsDraftManager();
            invoiceBulkActionsDraftManager.LoadInvoicesFromInvoiceBulkActionDraft(inputArgument.InvoiceBulkActionIdentifier, (invoiceQueue) =>
            {
                inputArgument.OutputQueue.Enqueue(invoiceQueue);
            });

        }
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<Entities.Invoice>());
            base.OnBeforeExecute(context, handle);
        }
        protected override LoadInvoiceBulkActionDraftInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadInvoiceBulkActionDraftInput()
            {
                InvoiceBulkActionIdentifier = this.InvoiceBulkActionIdentifier.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
    }
}
