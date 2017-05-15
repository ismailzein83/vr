using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class FinancialAccountRuntimeData
    {
        public long FinancialAccountId { get; set; }

        public string BalanceAccountId { get; set; }

        public Guid BalanceAccountTypeId { get; set; }
    }
}
