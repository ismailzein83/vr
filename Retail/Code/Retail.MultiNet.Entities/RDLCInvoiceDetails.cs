using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.MultiNet.Entities
{
    public class RDLCInvoiceDetails
    {
        public Decimal TotalAmount { get; set; }

        public decimal TotalDuration { get; set; }

        public int CountCDRs { get; set; }

        public RDLCInvoiceDetails()
        {

        }

        public IEnumerable<RDLCInvoiceDetails> GetRDLCInvoiceDetailsSchema()
        {
            return null;
        }
    }
}
