using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceQuery
    {
        public string PartnerId { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime? ToTime { get; set; }

    }
}
