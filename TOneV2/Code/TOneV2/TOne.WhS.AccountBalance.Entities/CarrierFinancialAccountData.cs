using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.AccountBalance.Entities
{
    public class CarrierFinancialAccountData
    {
        public int FinancialAccountId { get; set; }

        public Guid AccountTypeId { get; set; }

        public Guid UsageTransactionTypeId { get; set; }

        public Decimal? CreditLimit { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
