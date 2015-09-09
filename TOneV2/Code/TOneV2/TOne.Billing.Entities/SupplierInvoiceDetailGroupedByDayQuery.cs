using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Billing.Entities
{
    public class SupplierInvoiceDetailGroupedByDayQuery
    {
        public string SupplierID { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
