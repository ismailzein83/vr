using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Invoice.Business
{
    public class InvoiceSettings : SettingData
    {
        public const string SETTING_TYPE = "WhS_Invoice_InvoiceSettings";
        public List<InvoiceTypeSetting> InvoiceTypeSettings { get; set; }
    }
    public class InvoiceTypeSetting
    {
        public Guid InvoiceTypeId { get; set; }
        public bool NeedApproval { get; set; }
    }
}
