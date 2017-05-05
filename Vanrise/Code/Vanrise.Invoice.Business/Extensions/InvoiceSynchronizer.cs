using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business
{
    public class InvoiceSynchronizer : TargetBESynchronizer
    {
        public override string Name
        {
            get
            {
                return "Invoice Synchronizer";
            }
        }
        public bool IsUpdateOnly { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public override bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context)
        {
            Entities.Invoice invoice = GetInvoice(context);

            if (invoice != null)
            {
                context.TargetBE = new SourceInvoice
                {
                    Invoice = invoice
                };
                return true;
            }
            else if (IsUpdateOnly)
                context.WriteBusinessTrackingMsg(LogEntryType.Information, "Invoice '{0}' is not Updated", context.TargetBEId != null ? context.TargetBEId : context.SourceBEId);

            return false;
        }

        Entities.Invoice GetInvoice(ITargetBESynchronizerTryGetExistingBEContext context)
        {
            InvoiceManager invoiceManager = new InvoiceManager();
            long invoiceId = 0;
            Entities.Invoice invoice = null;

            if (context.TargetBEId != null)
            {
                long.TryParse(context.TargetBEId.ToString(), out invoiceId);
                invoice = invoiceManager.GetInvoice(invoiceId);
            }
            else if (context.SourceBEId != null)
            {
                invoice = invoiceManager.GetInvoiceBySourceId(InvoiceTypeId, context.SourceBEId.ToString());
            }
            else
                throw new NullReferenceException("SourceBEId");

            return invoice;
        }

        public override void InsertBEs(ITargetBESynchronizerInsertBEsContext context)
        {
            if (IsUpdateOnly)
                return;
            //TODO Insert Invoice
        }

        public override void UpdateBEs(ITargetBESynchronizerUpdateBEsContext context)
        {
            foreach (var targetBe in context.TargetBE)
            {
                SourceInvoice sourceInvoice = targetBe as SourceInvoice;
                InvoiceManager invoiceManager = new InvoiceManager();
                if (invoiceManager.TryUpdateInvoice(sourceInvoice.Invoice))
                {
                    context.WriteBusinessTrackingMsg(LogEntryType.Information, "Invoice '{0}' is Updated", sourceInvoice.SourceBEId);
                }
            }
        }
    }
}
