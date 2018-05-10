using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class FinancialAccountData
    {
        public string FinancialAccountId { get; set; }
        public FinancialAccount FinancialAccount { get; set; }

        public Account Account { get; set; }

        public bool IsInherited { get; set; }

        public Decimal? CreditLimit { get; set; }

        public int? CreditLimitCurrencyId { get; set; }

        public Guid? BalanceAccountTypeId { get; set; }

       // public Guid? InvoiceTypeId { get; set; }
        public List<Guid> InvoiceTypeIds { get; set; }

    }
}
