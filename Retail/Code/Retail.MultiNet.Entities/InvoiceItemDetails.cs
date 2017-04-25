using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.MultiNet.Entities
{
    public class InvoiceItemDetails
    {
        public Guid? ServiceTypeId { get; set; }
        public long? FinancialAccountId { get; set; }
        public decimal CountIN { get; set; }
        public decimal CountOUT { get; set; }
    }
}
