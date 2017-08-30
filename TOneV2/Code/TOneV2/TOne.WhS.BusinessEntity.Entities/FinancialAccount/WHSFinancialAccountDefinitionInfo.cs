using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class WHSFinancialAccountDefinitionInfo
    {
        public Guid FinancialAccountDefinitionId { get; set; }
        public string Name { get; set; }
        public Guid? BalanceAccountTypeId { get; set; }
        public Guid? InvoiceTypeId { get; set; }
    }
}
