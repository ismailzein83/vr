using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class WHSFinancialAccountToEdit
    {
        public int FinancialAccountId { get; set; }

        public WHSFinancialAccountSettings Settings { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
        public List<InvoiceSettingData> InvoiceSettingsData { get; set; }

        public DateTime CreatedTime { get; set; }

        public int? CreatedBy { get; set; }

        public int? LastModifiedBy { get; set; }

        public DateTime? LastModifiedTime { get; set; }
      
    }
}
