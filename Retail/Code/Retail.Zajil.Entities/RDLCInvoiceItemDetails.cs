using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Zajil.Entities
{
    public class RDLCInvoiceItemDetails
    {
        public Guid ServiceTypeId { get; set; }

        public string ServiceTypeIdDescription { get; set; }

        public long? ZoneId { get; set; }

        public string ZoneIdDescription { get; set; }

        public long? InterconnectOperatorId { get; set; }

        public string InterconnectOperatorIdDescription { get; set; }

        public decimal Amount { get; set; }

        public int CountCDRs { get; set; }

        public Decimal TotalDuration { get; set; }

        public RDLCInvoiceItemDetails()
        {
                
        }

        public IEnumerable<RDLCInvoiceItemDetails> GetRDLCInvoiceItemDetailsSchema()
        {
            return null;
        }
    }
}
