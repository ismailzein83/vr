using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceItemDetail
    {
        public List<InvoiceItemDetailObject> Items { get; set; }
        public InvoiceItem Entity { get; set; }
    }

    public class InvoiceItemDetailObject
    {
        public Object Value { get; set; }
        public string Description { get; set; }
    }
}
