using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class WHSFinancialAccountRuntimeEditor
    {
        public WHSFinancialAccount FinancialAccount { get; set; }
        public List<InvoiceSettingData> InvoiceSettingsData { get; set; }
    }
    public class InvoiceSettingData
    {
        public Guid InvoiceTypeId { get; set; }
        public Guid InvoiceSettingId { get; set; }
        public Guid PartnerInvoiceSettingId { get; set; }
    }
}
