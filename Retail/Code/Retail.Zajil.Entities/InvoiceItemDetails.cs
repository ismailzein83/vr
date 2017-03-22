using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Zajil.Entities
{
    public class InvoiceItemDetails
    {
        public decimal Amount { get; set; }

        public Guid? ServiceTypeId { get; set; }

        public long? ZoneId { get; set; }

        public long? InterconnectOperatorId { get; set; }

        public int CountCDRs { get; set; }

        public Decimal TotalDuration { get; set; }

    }
}
