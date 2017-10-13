using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Invoice.Business;
using Vanrise.BusinessProcess;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.BP.Activities
{
    public sealed class LoadInvoiceGenerationDrafts : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> InvoiceGenerationIdentifier { get; set; }

        [RequiredArgument]
        public OutArgument<List<InvoiceGenerationDraft>> InvoiceGenerationDraftList { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            Guid invoiceGenerationIdentifier = context.ActivityContext.GetValue(this.InvoiceGenerationIdentifier);
            InvoiceGenerationDraftManager invoiceGenerationDraftManager = new InvoiceGenerationDraftManager();
            List<InvoiceGenerationDraft> invoiceGenerationDraftList = invoiceGenerationDraftManager.GetInvoiceGenerationDrafts(invoiceGenerationIdentifier);
            this.InvoiceGenerationDraftList.Set(context.ActivityContext, invoiceGenerationDraftList);
        }
    }
}