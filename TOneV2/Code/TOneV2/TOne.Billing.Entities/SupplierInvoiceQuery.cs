using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Billing.Entities
{
    public class SupplierInvoiceQuery
    {
        public string selectedSupplierID { get; set; }
        public DateTime from { get; set; }
        public DateTime to { get; set; }
    }
}
