using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceBulkActionsDraftSummary
    {
        public int TotalCount { get; set; }
        public DateTime? MinimumFrom { get; set; }
        public DateTime? MaximumTo { get; set; }
    }
}
