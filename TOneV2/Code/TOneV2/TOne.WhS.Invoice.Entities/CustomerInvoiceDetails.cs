using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class CustomerInvoiceDetails
    {
        public int TotalNumberOfCalls { get; set; }
        public Double SaleAmount { get; set; }
        public decimal Duration { get; set; }
    }
}
