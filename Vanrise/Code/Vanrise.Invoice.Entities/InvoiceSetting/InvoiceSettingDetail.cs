using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceSettingDetail
    {
        public int TotalLinkedPartners { get; set; }
        public InvoiceSetting Entity { get; set; }
        public bool IsAutomatic { get; set; }
        public string BillingPeriodDescription { get; set; }
        public string DuePeriodDescription { get; set; }
        public bool CanDeleteInvoiceSetting { get; set; }
    }
}
