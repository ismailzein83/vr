using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class WHSFinancialAccountInfo
    {
        public int FinancialAccountId { get; set; }

        public WHSFinancialAccountCarrierType CarrierType { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public WHSFinancialAccountEffectiveStatus EffectiveStatus { get; set; }
        public Guid? BalanceAccountTypeId { get; set; }
        public Guid? InvoiceTypeId { get; set; }
    }
}
