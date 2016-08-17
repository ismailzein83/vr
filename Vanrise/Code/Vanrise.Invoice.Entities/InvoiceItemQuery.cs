using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceItemQuery
    {
        public long InvoiceId { get; set; }
        public string ItemSetName { get; set; }
    }
}
