using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.MultiNet.Entities
{
    public class InvoiceDetails
    {
        public Decimal TotalAmount { get; set; }

        public decimal TotalDuration { get; set; }

        public int CountCDRs { get; set; }
        public int DuePeriod { get; set; }
        public int CurrencyId { get; set; }
        public string VoiceCustomerNo { get; set; }
        public string SalesAgent { get; set; }
        public string CustomerPO { get; set; }
    }
}
