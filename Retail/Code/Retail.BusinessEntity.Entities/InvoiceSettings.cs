using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class InvoiceSettings : SettingData
    {
        public const string SETTING_TYPE = "Retail_Invoice_InvoiceSettings";
        public List<SubscriberInvoiceSettings> SubscriberInvoiceSettings { get; set; }
    }
    public class SubscriberInvoiceSettings
    {
        public Guid InvoiceSettingId { get; set; }
        public string Name { get; set; }
        public Guid DefaultEmailId { get; set; }
        public bool IsDefault { get; set; }
        public int DuePeriod { get; set; }
        public bool IsFollow { get; set; }
        public BillingPeriod BillingPeriod { get; set; }
        public string SerialNumberPattern { get; set; }
    }
}
