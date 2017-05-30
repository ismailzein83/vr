using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace PartnerPortal.Invoice.Entities
{
    public class InvoiceTile
    {
        public InvoiceClientDetail InvoiceDetail { get; set; }
        public string FormattedDate { get; set; }
        public string ViewURL { get; set; }
    }
}
