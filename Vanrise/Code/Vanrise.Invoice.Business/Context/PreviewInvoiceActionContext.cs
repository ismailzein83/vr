using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business
{
    public class PreviewInvoiceActionContext : IInvoiceActionContext
    {
        public Guid InvoiceTypeId { get; set; }
        public string PartnerId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public dynamic CustomSectionPayload { get; set; }
        private bool IsLoaded { get; set; }

        public void InitializeInvoiceActionContext()
        {
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(this.InvoiceTypeId);
            InvoiceGenerationContext context = new InvoiceGenerationContext
            {
                InvoiceTypeId = this.InvoiceTypeId,
                CustomSectionPayload = CustomSectionPayload,
                FromDate = this.FromDate,
                PartnerId = this.PartnerId,
                ToDate = this.ToDate
            };
            invoiceType.Settings.InvoiceGenerator.GenerateInvoice(context);
            this.GeneratedInvoice = context.Invoice;
            this._Invoice = new Entities.Invoice
            {
                Details = GeneratedInvoice.InvoiceDetails,
                ToDate = this.ToDate,
                PartnerId = this.PartnerId,
                FromDate = this.FromDate,
                InvoiceTypeId = this.InvoiceTypeId
            };
            this.IsLoaded = true;
        }
        public GeneratedInvoice GeneratedInvoice { get; set; }
        private Entities.Invoice _Invoice { get; set; }
        public Entities.Invoice GetInvoice
        {
            get
            {
                if (!this.IsLoaded)
                    InitializeInvoiceActionContext();
                return this._Invoice;
            }
        }
        public IEnumerable<InvoiceItem> GetInvoiceItems(List<string> itemSetNames)
        {
            if (!this.IsLoaded)
                InitializeInvoiceActionContext();
            
            List<InvoiceItem> invoiceItems = new List<InvoiceItem>();
            var generatedInvoiceItems = this.GeneratedInvoice.InvoiceItemSets.Where(x => itemSetNames.Contains(x.SetName));
            foreach (var generatedInvoiceItem in generatedInvoiceItems)
            {
                foreach (var item in generatedInvoiceItem.Items)
                {
                    invoiceItems.Add(new InvoiceItem
                    {
                        Details = item.Details,
                        ItemSetName = generatedInvoiceItem.SetName,
                        Name = item.Name,
                    });
                }
               
            }
            return invoiceItems;
        }
    }
}
