using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class WHSFinancialAccountToAdd
    {
        public WHSFinancialAccount FinancialAccount { get; set; }
        public List<InvoiceSettingData> InvoiceSettingsData { get; set; }

    }
}
