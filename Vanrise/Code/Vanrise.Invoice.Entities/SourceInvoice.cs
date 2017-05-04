using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;

namespace Vanrise.Invoice.Entities
{
    public class SourceInvoice : ITargetBE
    {
        public Invoice Invoice { get; set; }
        public object TargetBEId
        {
            get { return this.Invoice.InvoiceId; }
        }

        public object SourceBEId
        {
            get { return this.Invoice.SourceId; }
        }
    }
}
