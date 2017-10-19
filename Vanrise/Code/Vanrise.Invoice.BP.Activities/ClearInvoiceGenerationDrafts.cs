using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Invoice.Business;
using Vanrise.BusinessProcess;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.BP.Activities
{
    public sealed class ClearInvoiceGenerationDrafts : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> InvoiceGenerationIdentifier { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            Guid invoiceGenerationIdentifier = context.ActivityContext.GetValue(this.InvoiceGenerationIdentifier);
            InvoiceGenerationDraftManager invoiceGenerationDraftManager = new InvoiceGenerationDraftManager();
            invoiceGenerationDraftManager.ClearInvoiceGenerationDrafts(invoiceGenerationIdentifier);
        }
    }
}