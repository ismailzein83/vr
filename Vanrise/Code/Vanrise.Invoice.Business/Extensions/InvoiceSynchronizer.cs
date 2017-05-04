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
        public bool ByOriginalId { get; set; }
        public bool IsUpdateOnly { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public override bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context)
        {
            InvoiceManager invoiceManager = new InvoiceManager();
            Entities.Invoice invoice = ByOriginalId ? invoiceManager.GetInvoice(long.Parse(context.SourceBEId.ToString())) : invoiceManager.GetInvoiceBySourceId(InvoiceTypeId, context.SourceBEId.ToString());
            if (invoice != null)
            {
                context.TargetBE = new SourceInvoice
                {
                    Invoice = invoice
                };
                return true;
            }
            return false;
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
