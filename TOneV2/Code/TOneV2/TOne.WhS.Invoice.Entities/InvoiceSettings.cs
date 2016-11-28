using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class InvoiceSettings : Vanrise.Entities.SettingData
    {
        public const string SETTING_TYPE = "WhS_Invoice_InvoiceSettings";
        public CustomerInvoiceSettings CustomerInvoiceSettings { get; set; }
    }
    public class CustomerInvoiceSettings 
    {
        public Guid DefaultEmailId { get; set; }
    }
}
