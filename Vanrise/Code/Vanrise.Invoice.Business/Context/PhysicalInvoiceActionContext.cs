﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business
{
    public class PhysicalInvoiceActionContext : IInvoiceActionContext
    {
        public long InvoiceId { get; set; }
        private bool IsLoaded { get; set; }
        public void InitializeInvoiceActionContext()
        {
            this.IsLoaded = true;
        }
        private Entities.Invoice _Invoice { get; set; }
        public Entities.Invoice GetInvoice
        {
            get
            {
                if (this._Invoice == null)
                {
                    InvoiceManager invoiceManager = new Business.InvoiceManager();
                    return invoiceManager.GetInvoice(this.InvoiceId);
                }
                return this._Invoice;
            }
        }
        public IEnumerable<Entities.InvoiceItem> GetInvoiceItems(List<string> itemSetNames)
        {
            InvoiceItemManager invoiceItemManager = new InvoiceItemManager();
            return invoiceItemManager.GetInvoiceItemsByItemSetNames(this.InvoiceId, itemSetNames);
        }
    }
}
