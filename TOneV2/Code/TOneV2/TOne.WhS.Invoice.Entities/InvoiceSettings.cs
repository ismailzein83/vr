using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;
using Vanrise.Invoice.MainExtensions;

namespace TOne.WhS.Invoice.Entities
{
    public class InvoiceSettings : Vanrise.Entities.SettingData
    {
        public const string SETTING_TYPE = "WhS_Invoice_InvoiceSettings";
        public List<CustomerInvoiceSettings> CustomerInvoiceSettings { get; set; }
    }
    public class CustomerInvoiceSettings 
    {
        public string Title { get; set; }
        public Guid DefaultEmailId { get; set; }
        public bool IsDefault { get; set; }
        public int DuePeriod { get; set; }
        public bool IsFollow { get; set; }
        public BillingPeriod BillingPeriod { get; set; }
        public string SerialNumberPattern { get; set; }
    }
}
