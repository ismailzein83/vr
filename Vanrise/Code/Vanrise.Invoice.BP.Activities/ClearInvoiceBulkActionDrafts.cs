using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Invoice.Business;
using Vanrise.BusinessProcess;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.BP.Activities
{
    public sealed class ClearInvoiceBulkActionDrafts : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> InvoiceBulkActionIdentifier { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            context.ActivityContext.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Please wait a little to complete execution...");
            Guid invoiceBulkActionIdentifier = context.ActivityContext.GetValue(this.InvoiceBulkActionIdentifier);
            InvoiceBulkActionsDraftManager invoiceBulkActionsDraftManager = new InvoiceBulkActionsDraftManager();
            invoiceBulkActionsDraftManager.ClearInvoiceBulkActionDrafts(invoiceBulkActionIdentifier);
        }
    }
}