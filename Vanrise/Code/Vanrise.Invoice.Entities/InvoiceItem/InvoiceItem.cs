using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceItem
    {
        public long InvoiceItemId { get; set; }

        public long InvoiceId { get; set; }

        public string ItemSetName { get; set; }

        public string Name { get; set; }

        public dynamic Details { get; set; }
      
    }
   
}
